# Logging
This service uses serilog to do logging and sends logs to elasticsearch,
You can obviously not set it up and have it just display logs on console, but setting up elastic search with kibana is highly recommended to be able to debug issues.

`ElasticConfiguration` accepts host, user and password for the elastic search user, for now only 1 node is supported, but multiple node support is under consideration as well.
