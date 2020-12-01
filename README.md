# autobakr
autobakr is an automated bread detection program for bakr.io

## About
autobakr takes in user-based input to process a number of input phrases and files containing input phrases to search for in sentences. These sentences are given by the user also, or can be read from a file.

## Considerations
An assumption is that these files that contain phrases and sentences are split by line. Given the text files provided were also single-sentence, single-line files, I feel this is a suitable assumption to make.
Tested with all of the text files. 

autobakr uses console and is user-input based. This can be easily used to accept and output data to/from other .NET applications. Runs on .NET 5.
