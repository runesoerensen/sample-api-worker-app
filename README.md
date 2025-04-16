# Sample Web API and Worker App with Remote Debugging

This sample application demonstrates how to build and deploy a .NET Web API with background job processing using a worker dyno on Heroku.

It also includes configuration for remote debugging using Visual Studio Code.

## Features

- Web API endpoint for triggering email jobs
- Background job processing using a worker dyno
- Message queue integration using CloudAMQP (RabbitMQ)
- Remote debugging support with Visual Studio Code

## Prerequisites

- [Heroku CLI](https://devcenter.heroku.com/articles/heroku-cli)
- [Visual Studio Code](https://code.visualstudio.com/)
- [.NET SDK](https://dotnet.microsoft.com/download)
- Git

## Deployment Instructions

1. Clone the repository:
   ```bash
   git clone https://github.com/runesoerensen/sample-api-worker-app.git
   cd sample-api-worker-app
   ```

2. Create a new Heroku app:
   ```bash
   heroku create
   ```

3. Add the CloudAMQP add-on for message queuing:
   ```bash
   heroku addons:create cloudamqp:lemur
   ```

4. Configure the build configuration for remote debugging:
   ```bash
   heroku config:set BUILD_CONFIGURATION=Debug
   ```

5. Deploy the application:
   ```bash
   git push heroku main
   ```

6. Scale the application to run both web and worker dynos:
   ```bash
   heroku ps:scale web=1 worker=1
   ```

## Setting Up Remote Debugging

1. Enable Heroku Exec for remote debugging:
   ```bash
   heroku ps:exec
   ```
   Enter `y` when prompted to continue. This will restart your dynos and prepare them for remote debugging connections.

2. Open the project in Visual Studio Code:
   - Open the `sample-api-worker-app` directory
   - The included `.vscode/launch.json` file contains the necessary debugging configuration
   - The `Directory.Build.props` file ensures the debugger is installed in a shared folder

3. Start a debugging session:
   - Open `EmailWorker/Worker.cs`
   - Set a breakpoint in the `ProcessEmailRequestAsync` method
   - Press F5 or select "Run" -> "Start Debugging"
   - When prompted, enter `worker.1` as the Heroku Dyno name

## Testing the Application

Once the debugger is attached, you can test the email processing by sending a request:

```bash
curl -X POST https://YOUR-APP-NAME.herokuapp.com/send-email \
  -H "Content-Type: application/json" \
  -d '{"To": "user@example.com", "Subject": "Hello!", "Body": "This is a test email."}'
```

Replace `YOUR-APP-NAME` with your Heroku app's URL.

## Project Structure

- `Api/`: Web API project handling HTTP requests
- `EmailWorker/`: Background worker processing email requests
- `Shared/`: Shared code and models
- `.vscode/`: VS Code configuration including launch settings
- `Directory.Build.props`: Debugger installation configuration
- `Procfile`: Heroku process type declarations

## Important Notes

- The application must be built in `Debug` configuration to enable remote debugging
- The included VS Code launch configuration file and `Directory.Build.props` can be copied to other .NET applications for remote debugging.
- It's recommended to test the setup with this sample app before implementing in your own applications
