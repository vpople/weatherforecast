# Weather Forecast App!
This is sample application for demonstrating integration between AWS, Docker and MySQL.

Apps
 - Web App (Deployed in S3 bucket)
 - Web API (Deployed in AWS ECS Container)
 - MySQL   (AWS Arora DB)

## Prerequisits

Below are the prerequisits for local development:
 - Docker
 - Dotnet Core
 - aws-cli

Prerequisits for deployment:
 - Docker
 - aws-cli (with AWS configured credentials and Cloudformation deployment permissions)

## Deployment on local system
Below command can be used to deploy project on local system:

    > docker-compose up -d --build --force-recreate
On local, this will deploy 3 containers for Wep App, Web API and MySQL respectively.

## Deployment for AWS Account
Deployment for this application is configured using Powershell scripts (`ps_deployement_scripts.ps1`) provided inside the deployment directory.
 
Below variables can be cofigured inside the Powershell script:
 -  **baseStackName**: Stack name for deploying the base components like S3 bucket for Web App, S3 bucket for storing artifacts for Web App and repository inside ECR
 -  **rdsStackName**: Stack name for deploying RDS and ECS 
 -  **region**: AWS Region in which cloudformations will be deployed.
 - **profileOption**: AWS Profile name configured on local system.
 
 Deploy the stacks using below command:

    > cd ForecastWebApi  // Goto project root directory
    > .\deployment\ps_deployement_scripts.ps1 \\ Execute powershell

 
The file explorer is accessible using the button in left corner of the navigation bar. You can create a new file by clicking the **New file** button in the file explorer. You can also create folders by clicking the **New folder** button.