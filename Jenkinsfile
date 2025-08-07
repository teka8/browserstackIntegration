pipeline {
    agent any

    parameters {
        string(
            name: 'TAG_FILTER',
            defaultValue: '@smoke or @automation',
            description: 'Enter Reqnroll tags to run (e.g. @smoke or @regression or @smoke and @login)'
        )
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
                    def tagInput = TAG_FILTER?.trim()
                    def tagArg = ""

                    if (tagInput) {
                        // Process the tag filter into NUnit format
                        def parsed = tagInput
                            .replaceAll("@", "")                             // remove @
                            .replaceAll("(?i)\\band\\b", "&")                // "and" -> "&"
                            .replaceAll("(?i)\\bor\\b", "|")                 // "or" -> "|"
                            .split("\\s+")
                            .collect { "TestCategory=${it}" }
                            .join(' ')

                        tagArg = "--filter \"(${parsed})\""
                    }

                    bat "dotnet test --logger:\"trx\" ${tagArg}"
                }
            }
        }
    }

    post {
        always {
            // If you generate .trx files and want NUnit plugin:
            nunit '**/TestResults/*.xml'

            // Or if you're using trx output only, use this instead:
            // junit '**/TestResults/*.trx'
        }
    }
}
