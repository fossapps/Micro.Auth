# Emails

Auth Service depends on being able to sending email for various purposes like: verifying user's account, resetting password, etc

To do that, we need to configure email service

Only sending via SMTP is supported for now, and adding a email service so you can use other services with HTTP API is under consideration.

One email service will be written which will accept a HTTP endpoint and send email via SMTP, and it'll be upto consumer to fork the repo and create other services (and maybe opensource them)

Currently you can configure email service, it accepts smtp information and sender's information

