Docker desktop and Dapr CLI are required.
Component set could be default

#event_aggregator
cd Services\EventAggregator
dapr run --app-id event_aggregator --app-port 5003 --dapr-http-port 65391 --dapr-grpc-port 65392 -- dotnet run

#stereotype_recognizer
Services\StereotypeRecognizer
dapr run --app-id stereotype_recognizer --app-port 5002 --dapr-http-port 58002 --dapr-grpc-port 58003 -- dotnet run

#Dashboard
cd Dashboard
dapr run dotnet run
*.edison files in examples directory could be used for testing

#WorkerManager
cd WorkerManagerService\WorkerManager.Actors
dapr run --dapr-http-port 3505 --app-id worker_manager_actors --app-port 5005 --dapr-http-port 3505 --dapr-grpc-port 52250 dotnet run

#TestWorker
cd TestWorker
dapr run --app-id workers dotnet run {count of workers}
