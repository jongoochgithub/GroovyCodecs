# GroovyMp3
A C# native implementation of an MP3 encoder, based on LAME and Jump3r

Released under the LGPL v3.0.

Compiles to a .NET Standard 2.0 DLL and will run on .NET Framework >= 4.6 and .NET Core >= 2.0.

Fully managed code, no external OS dependencies, cross platform.

Not the most polished or optimised code; if you want efficiency you should be using LAME via NAudio or something like that.

Some of the Jump3r java code was converted to C# using the Free Edition of the Java to C# Converter produced by Tangible Software Solutions - https://www.tangiblesoftwaresolutions.com

V0.1.0 (alpha)

Functional MP3 encoding
A minimal console test app that will convert a wav file to an mp3 file
NOTE: this is still alpha code, and the Mp3Sharp.dll interface is likely to change
