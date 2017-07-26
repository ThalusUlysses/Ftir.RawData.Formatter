# Foss (Winescan) FTIR Raw Data Fromatter
The FTIR raw data formatter is able to apply formats to existing raw data
for Foss Winescan(R) *.csv* files. Basically the output data looks something like this:

```
Job name: 160802_Zatopack P30 (1)
Collection date: 02.08.2016
Job type: NORMAL(MULTIINTAKE)

Probe;Dichte;Gluc;Fruct;pH;Säure;Ws.;Äs.;fl.Sre.;Glucons.;Alk;Glyc;NH4;NOPA;Datum;Zeit;Produkt;Remark;Type;SubType

57a;1,0190;8,4;4,9;2,80;26,5;12,2;16,0;0,99;0,0;0,0;0,4;245;236;02.08.2016;13:24:37;GrapeScan Analytik;;NORMAL(MULTIINTAKE);VALUE

Job name: 160802_Zatopack P30 (2)
Collection date: 02.08.2016
Job type: NORMAL(MULTIINTAKE)

Probe;Dichte;Gluc;Fruct;pH;Säure;Ws.;Äs.;fl.Sre.;Glucons.;Alk;Glyc;NH4;NOPA;Datum;Zeit;Produkt;Remark;Type;SubType
558;1,0190;8,4;4,9;2,80;26,5;12,2;16,0;0,99;0,0;0,0;0,4;245;236;02.08.2016;13:24:37;GrapeScan Analytik;;NORMAL(MULTIINTAKE);VALUE
```

The above is somehow humanized but far from beeing a valid csv at all. The formatter 
switches the "Headlines" such as "Job Name" to columns and prettifies it for beeing
machine readable as "real" csv.

# How to to use the fromatter
The formatter is a small console based tool which makes the described transformation.

```
FTIR CSV Formatter 1.1.0.0
Copyright c  2017

  -f, --files        Formats a set of data files into to valid csv files.
                     Examle: Ftir.Csv.Formatter.exe -a
                     "C:\temp\160802_GrapeScan P30.csv"
                     "C:\temp\160802_GrapeScan P31.csv"
                     "C:\temp\160802_GrapeScan P32.csv"

  -d, --directory    Formats all files within a directory to valid csv files.
                     Example Ftir.Csv.Formatter.exe -d
                     "C:\temp\160802_GrapeScan"

  -k, --myLocales    (Default: False) Changes the separator char and the
                     numerical separator my locales (e.g '.' to ',')

  -c, --columns      Restrict output data to columns. Example: "Probe" "F-SO2"

  --help             Dispaly this help screen.
```
Formatted *.csv* files are put in a folder "Formatted" next to the file location
with the same name as the source file.
## Double Click
Using the fromatter as "Double Click" executable it will perfom a format / 
transformation to all csv file that are located next to the executable.
## Files
Using the formatter executable to perform format / transformation to a set
of files. Pass the full qualifed path as arguments to the executable.
```
Ftir.Csv.Formatter.exe -f "<file1.csv>" "file2.csv"
```
## Directory
Using the formatter executable to perform format / transformation to files
within a directory which is not the root directory of your formatter executable,
Pass the directory name as argument.
```
Ftir.Csv.exe -d "c:\temp\<mycoolsamples>"
```
## Keep the csv format
When exchanging csv over cultural habitats such like En-Us for US-America or De-En
where diffrent language settings for ',' or '.' appear there is an option that prevents
converting ',' to '.' and vice versa.
```
Ftir.Csv.exe -d "c:\temp\<mycoolsamples>" -k
```
with the *-k* option the ',' or '.' (and ',' or ';') is kept according to the
chars used in the source files. The formatter executable is able to detect 
weather it is ',' and ';' or '.' and ','.
