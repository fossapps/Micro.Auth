# Dependencies
The following dependencies are required for this auth service to run optional dependencies will be marked accordingly

### Postgres Database
This service works with postgres database, in order to deploy this, you'll need database host, user which can connect to the database, password and the name of database it's supposed to connect to.

Configuration of database is easy, you can find the related configuration in appsettings.json and it's also possible to configure using environment variables as demonstrated in Changing Configuration section of documentation

### FossApps/Micro.KeyStore
The best part of this service is that private keys never even touch secondary storage devices like hard drive or ssd, they exist on memory, and there's no way to access those, and public key is used to sign the JWT which are stored in Micro.KeyStore service. Since these keys are rotated quite often as a security measure, we need a way to store the keys with some retention policy.

For now only storing in Micro.KeyStore is supported, but it can be modified to save to other storage such as S3 as well.

Configuration of this service is also very easy, simply change the url under Services.KeyStore in appsettings, alternatively setting `Services__KeyStore__Url="<your new value>"` as your environment variable also works.

***Please note: In under no circumstances KeyStore is to be made accessible by public, it's supposed to run on your private network without it being able to hit by external traffic, that means if you're using kubernetes, there must be no ingres setup for keystore, and if you're using docker-compose to deploy, port must not be mapped, auth service needs to talk to keystore directly and internally***

### InfluxDb
For sending measurement messages, InfluxDb is setup, this is an optional dependency although it's highly recommended, if you do deploy this, configuring this is same as any other dependencies, you can then use grafana to query metrics and setup alerts.

### ElasticSearch
For logging, this service uses ELK stack (though it doesn't have logstash yet), it sends all logs using serilog to elastic search, you can then use kibana to query logs
Configuration for elastic search is also same as every other dependency,
