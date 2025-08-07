pipeline {
    agent any

    parameters {
        string(name: 'TAG_FILTER', defaultValue: '', description: '@smoke')
    }

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
                script {
                    def tagArg = TAG_FILTER?.trim() ? "--filter TestCategory=${TAG_FILTER.replace('@','')}" : ""
                    bat "dotnet test --logger:\"trx\" ${tagArg}"
                }
            }
        }
    }

    post {
        always {
            nunit '**/TestResults/*.xml'
        }
    }
}
