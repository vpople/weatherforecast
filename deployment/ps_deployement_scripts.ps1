$baseStackName = "base-infra"
$region = "eu-west-1"
$rdsStackName = "rds-weather-info"
$profileOption = "default"

#deploy s3 bucket & ECR cloudformation script
aws cloudformation deploy --template-file ./deployment/cf-bucket-creation.yaml --stack-name $baseStackName --profile $profileOption

# Rerieve information about Elastic Stack
$cmd = "aws cloudformation describe-stacks --stack-name $baseStackName --profile $profileOption";
Write-Host $cmd
$result = (Invoke-Expression $cmd)
$outputs = ($result | ConvertFrom-Json)
$webAppBucketName = ($outputs.Stacks.Outputs | Where-Object { $_.ExportName -eq "WebAppBucketName" }).OutputValue
$webAPIBucketName = ($outputs.Stacks.Outputs | Where-Object { $_.ExportName -eq "WebAPIBucketName" }).OutputValue
$ecrRepositoryURI = ($outputs.Stacks.Outputs | Where-Object { $_.ExportName -eq "ECRRepositoryURI" }).OutputValue
$ecrRepositoryName = ($outputs.Stacks.Outputs | Where-Object { $_.ExportName -eq "ECRRepositoryName" }).OutputValue
Write-Host "WebAppBucketName: $webAppBucketName"
Write-Host "WebAPIBucketName: $webAPIBucketName"
Write-Host "ecrrepositoryname: $ecrRepositoryName"


#uploading web API build to s3 bucket
# aws s3 cp ../ForecastWebApi/bin/Release/ s3://$webAPIBucketName/build/ --recursive

#Docker commands ECR login
# aws ecr get-login-password $profileOption | docker login --username AWS --password-stdin $ecrRepositoryURI

#Docker command for WEB API Build 
docker build -f ./ForecastWebApi/Dockerfile --tag weather-info-api:dev .

#tag docker image 
docker tag weather-info-api:dev $ecrRepositoryURI/$ecrRepositoryName

#Docker command for doker Image Push 
docker push $ecrRepositoryURI/$ecrRepositoryName

$imageURI = ($ecrRepositoryURI + "/"  + $ecrRepositoryName + ":latest")
Write-Host "imageURI: $imageURI"

#deploy RDS cloudformation script
aws cloudformation deploy --profile $profileOption --template-file ./deployment/cf-RDS-Aurora-creation.yaml --stack-name $rdsStackName --capabilities CAPABILITY_NAMED_IAM --no-fail-on-empty-changeset --parameter-overrides ImagePath=$imageURI

$cmd = "aws ecs list-tasks --cluster ECSCluster --profile $profileOption"
$result = (Invoke-Expression $cmd)
$outputs = ($result | ConvertFrom-Json)
$taskArn = ($outputs.taskArns[0])
Write-Host "taskArn: $taskArn"

$cmd = "aws ecs describe-tasks --cluster ECSCluster --tasks $taskArn --profile $profileOption"
$result = (Invoke-Expression $cmd)
$outputs = ($result | ConvertFrom-Json)
$eniId = ($outputs.tasks.attachments.details | Where-Object { $_.name -eq "networkInterfaceId" }).value
Write-Host "eniId: $eniId"

$cmd = "aws ec2 describe-network-interfaces --network-interface-ids $eniId --profile $profileOption"
$result = (Invoke-Expression $cmd)
$outputs = ($result | ConvertFrom-Json)
$publicIp = ($outputs.NetworkInterfaces.Association.PublicIp)
Write-Host "publicIp: $publicIp"

$filePath = "./SinglePageWebApp/index-dev.html"
$filePathDes = "./SinglePageWebApp/build/index.html"
new-item -itemtype file -path $filePathDes -f
(Get-Content -path $filePath -Raw) -replace '#DomainURL',$publicIp | Set-Content $filePathDes

#uploading web app build to s3 bucket
aws s3 cp ./SinglePageWebApp/build s3://$webAppBucketName/ --recursive --profile $profileOption

Remove-Item –path $filePathDes
