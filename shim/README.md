# Shim Reference Source Code

<!-- TOC -->

- [Summary](#summary)
- [General](#general)
- [Notes](#notes)

<!-- /TOC -->

### Summary

While ShimGenerator (shimgen) will remain closed source, we are committed to showing you what it produces so you can verify with your security team that the code is safe.

### General

In this folder you will find the files that are used to create a shim. This will allow you to

* Verify that nothing nefarious is going on - you can verify this by finding a shim file and using something like ILSpy to inspect the file itself to see it is the same
* See how the shim works
* Be able to file issues on something incorrect with the shim itself by knowing the source itself.
* Build something from the reference source to use for internal purposes.

### Notes

Note the LICENSE granted to these source files is Ms-RSL (Microsoft Reference Source License). This means you can reference the source only.
