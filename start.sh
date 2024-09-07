#!/bin/bash

# Add ngrok authtoken
ngrok authtoken $NGROK_AUTHTOKEN

# Start ngrok in the background
ngrok http 8080 &

# Wait for ngrok to generate a public URL
sleep 5

# Get the ngrok public URL
export NGROK_URL=$(curl -s http://localhost:4040/api/tunnels | jq -r '.tunnels[0].public_url')

echo "ngrok URL: $NGROK_URL"

# Start the .NET application
dotnet MonkeyIsland.dll