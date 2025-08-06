pipeline {
    agent any

    environment {
        BROWSERSTACK_USERNAME = credentials('browserstack-username')
        BROWSERSTACK_ACCESS_KEY = credentials('browserstack-key')
    }

    stages {
        stage('Checkout') {
            steps {
                git 'https://github.com/teka8/browserstackIntegration.git'
            }
        }

        stage('Restore') {
            steps {
                bat 'dotnet restore'
            }
        }

        stage('Build') {
            steps {
                bat 'dotnet build'
            }
        }

        stage('Test') {
            steps {
                bat 'dotnet test --logger:"trx"'
            }
        }
    }

    post {
        always {
            nunit '**/TestResults/*.xml'
        }
    }
}
