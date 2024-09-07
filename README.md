# Monkey Island Technical Challenge

## Overview

This repository contains the solution for the Monkey Island technical challenge.

## Setup Instructions

### Prerequisites

- **Docker**: Ensure Docker is installed and running on your local machine.

### Clone the Repository

1. Clone this repository to your local machine:

    ```sh
    git clone https://github.com/AldoBusiness/MonkeyIsland.git
    cd MonkeyIsland
    ```

### Environment Variables

2. Create a `.env` file in the root directory with the following variables:

    ```env
    NGROK_AUTHTOKEN=your_ngrok_authtoken
    API_KEY=your_personal_api_key
    ```

### Building and Running

3. **Build the Docker Image**:

    ```sh
    docker build -t monkey-island .
    ```

4. **Run the Docker Container**:

    ```sh
    docker run --env-file .env -p 8080:8080 monkey-island
    ```

5. **Check the Console Logs**:
  - After running the Docker container, check the console logs: the received key will be printed here.

