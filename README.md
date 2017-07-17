# sms-survey-csharp
Simple back and forth SMS survey application using the Bandwidth C# SDK

Current Version Details:

This C# application is a very basic example of the ability to send SMS surveys using the Bandwidth C# SDK. Currently, sms resposnses to answers are not saved, this application solely demonstrates the ability to load survey questions from a text document and automate the sending of these questions to a desired phone number. Instead of an http server, a requestb.in URL is used. This version also does not permit the ability to send multiple messages without a response. Since no server is used, a message will be sent from the Bandwidth catapult account and wait for a response from the desired survey-taker's phone number. Once a response is made, the survey application will automatically send the next question. 
