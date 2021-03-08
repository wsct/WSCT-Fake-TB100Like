# ISO/IEC 7816 Smartcard Applet

## Objectives

Simulate a smart card reader connected to a TB100 like smart card for exclusive use with [WSCT core framework](https://github.com/wsct/WSCT-Core).

It's based on a port of an previously developed javacard applet: [TB100 like card](https://github.com/wsct/ENSICAEN-Card-Applet/tree/develop) and some tweaks to simulate a T=0 javacard runtime environment.

## Unit testing

Beeing developed in a hurry for teaching purposes during the covid-19, please don't complain if you can't find much (if any) unit tests ;-) .<br/>
 _"Do as I say, not as I do."_

# The TB100 Like Applet

## Supported APDU

  * [x] SELECT
  * [x] CREATE FILE
  * [x] DELETE FILE
  * [x] GENERATE RANDOM
  * [x] READ BINARY
  * [x] WRITE BINARY
  * [x] ERASE

# The WSCT.Fake.JavaCard framework

This framework aims to provide an API indentical to the JavaCard one for easy port of JavaCard applets to C#.

## Fake JavaCard
Current mocked JavaCard classes and methods are based my own needs so expect not to see all the JavaCard framework to be mocked.

  * **`javacard.framework`** (in namespace `WSCT.Fake.JavaCard.Framework`)
    * `APDU`
    * `Applet`
    * `ISO7816`
    * `ISOException`
    * `JCSystem`
    * `Util`
    * **`security`** (in namespace `Security`)
      * `Cipher`
      * `Key`
      * `KeyBuilder`
      * `MessageDigest`
      * `RandomData`
      * `SecureRandom`
      * `Signature`

## How to

### Adapt a JavaCard applet to C#
  1. Copy the JavaCard code (`.java` files) to `.cs` files.
  2. Replace JavaCard `import`s with the corresponding `using`s of WSCT.Fake namespaces.
  3. Replace `package` clauses with `namespace` clauses.
  4. Replace `.length` by `.Length` on java arrays.
  5. Replace `static final`s with `const` for `byte` and `short` typed constants.
  6. Replace `boolean`s with `bool`s.
  7. Remove `final` from method signatures.
  8. REplace `ISO7816.` with `JavaCard.ISO7816` (there's a conflict between `WSCT.ISO7816` namespace and `WSCT.Fake.JavaCard.ISO7816` class)
  9. Enclose `(short)` cast where bit 31 (most significative bit) is set 1 in a `unchecked(...)` clause.
  10. In `install`method, remove the applet instanciation (the mechanism in not yet implemented)
  11. Optionaly: add an attribute to suppress IDE1006 warning because JavaCard uses the camelCase coding style.
  ```[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "JavaCard")]```

## Use a migrated applet in WSCT
```cs
var FakeCard = new FakeJavaCard("F0 43 41 45 4E 42 01".FromHexa(),new EmvApplet())
```

## Use several migrated applets in WSCT

```cs
HostedApplet[] hostedApplets =
{
    new HostedApplet("F0 43 41 45 4E 42 01".FromHexa(),new EmvApplet()),
    new HostedApplet("1PAY.SYS.DDF01".FromString(), new PseApplet())
};
var FakeCard = new FakeJavaCard(hostedApplets)
```