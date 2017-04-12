# AWSSDK SNS (.Net Core) support package

When using AWS .Net SDK on .Net Core, some function are not supported due to platform specific library,
one of important function is SNS message signature validtion, this package is deisgned to add back this function, and maybe more later.

## Install
Manually -
https://www.nuget.org/packages/AWSSDK.SimpleNotificationService.NetCore.Support/

Package Manager -
Install-Package AWSSDK.SimpleNotificationService.NetCore.Support


## Usage
```
using Amazon.SimpleNotificationService.Util;

Message message = Message.ParseMessage(data);
bool valid = message.IsMessageSignatureValid();
```

As you see, just call `IsMessageSignatureValid()` on message instance.


## About
Welcome to any contribution :)
