#!/usr/bin/env bash
echo '{"values": [{"key": "url","value": "http://localhost:5000"}]}' > env.json
npx newman run https://www.getpostman.com/collections/38b3855cdbfaf9a33dc1 -e ./env.json && rm env.json
