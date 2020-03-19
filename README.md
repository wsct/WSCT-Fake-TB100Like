# ISO/IEC 7816 Smartcard Applet

## Objectives

Simulate a smart card reader connected to a TB100 like smart card for exclusive use with [WSCT core framework](https://github.com/wsct/WSCT-Core).

It's based on a port of an previously developed javacard applet: [TB100 like card](https://github.com/wsct/ENSICAEN-Card-Applet/tree/develop) and some tweaks to simulate a T=0 javacard runtime environment.

## Unit testing

Beeing developed in a hurry for teaching purposes during the covid-19, please don't complain if you can't find much (if any) unit tests ;-) .<br/>
 _"Do as I say, not as I do."_

## Supported APDU
  * SELECT
  * CREATE FILE
  * DELETE FILE
  * GENERATE RANDOM
  * READ BINARY
  * WRITE BINARY
  * ERASE
