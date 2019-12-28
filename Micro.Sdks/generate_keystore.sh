#!/usr/bin/env bash
mkdir -p KeyStore
docker run --net=host -it --rm -v $(pwd)/KeyStore:/app/sdk node bash -c "apt update && apt install libunwind-dev -y && npm i -g autorest && autorest --input-file=http://localhost:15000/swagger/v1/swagger.json --csharp --namespace=FossApps.KeyStore --override-client-name=KeyStoreClient --output-folder=/app/sdk --clear-output-folder"
