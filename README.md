# webwatcher
A command line application to watch periodically a website to check if it has changed. If changed, it will beep for a while and show through console the differences in the HTML.

## Launch instrutions
To launch it, run:
dotnet webwatcher.dll.

### Arguments
They can be passed on order or they will be asked after the launch through console.

{0} : the url to check periodically

{1} : a css selector to specify only a subsection of the document t compare periodically

Example:

dotnet webwatcher.dll https://www.github.com .pagehead
