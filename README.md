# Raw File Compare

##### Overview

`rawcmp` is a command-line tool that compares raw content of two files, ignoring metadata and other insignificant data (archive comments, MP3 tags, image thumbnails, etc.)

This tool is part of [`dostools` collection](https://github.com/vurdalakov/dostools).

##### Supported file formats

  * [ZIP](https://github.com/vurdalakov/rawcmp/wiki/ZIP) - compares only file names, size and CRC.

##### Syntax

```
rawcmp file1 file2 [-silent]
```

`-silent` option tells tool not to print anything to stdout; check exit code for comparison result.

##### Exit codes

  * 0 - files are equal;
  * 1 - files are different;
  * 2 - one or both files not found;
  * 3 - unsupported file format;
  * -1 - invalid command line syntax.

##### License

Copyright © 2015 [Vurdalakov](http://www.vurdalakov.net).

Project is distributed under the [MIT license](http://opensource.org/licenses/MIT).
