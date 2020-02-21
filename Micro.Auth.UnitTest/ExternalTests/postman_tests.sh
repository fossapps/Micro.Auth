#!/usr/bin/env bash
npx newman run https://www.getpostman.com/collections/38b3855cdbfaf9a33dc1 -e ./env.json
retVal=$?
cd ../../
docker-compose logs
exit $retVal
