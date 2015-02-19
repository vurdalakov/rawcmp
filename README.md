# Raw File Compare

`rawcmp` command-line tool compares raw content of two files, ignoring insignificant data and metadata (archive comments, MP3 tags, image thumbnails, etc.)

##### Supported file formats

  * ZIP - compares only file names, size and CRC.

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
