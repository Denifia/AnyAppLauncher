# AnyAppLauncher
Allows you to open any application on your computer from a simple URI.

> This app was created as a way to open Chrome from a URL even though IE was the default browser.

## Installation
Simply download the app and run it to install. It registers itself as a URI Scheme handler with Windows.

## How to use
Create a URI with the following format:
<pre>
 anyapp://chrome:https@example.com:8042/over/there/index.dtb?type=animal
 \____/   \____/ \___/ \_________/ \__/ 
   |        |      |        |       |    
 scheme    app    new     host     port  
  name           scheme
</pre>
Note: The above example of a URI was based on the [URI scheme](https://en.wikipedia.org/wiki/URI_scheme) wiki page.

The above URI is converted to this new URI:
<pre>
https://example.com:8042/over/there/index.dtb?type=animal
</pre>

Chrome is then opened with the new URI as the argument; opening the website.

This is based of the default conversion format and configuration.

```html
<applicationSection>
  <applications>
    <add
      name="chrome"
      path="HKEY_LOCAL_MACHINE\Software\Microsoft\Windows\CurrentVersion\App Paths\Chrome.exe"
      pathType="1"/>
  </applications>
</applicationSection>
```

To open the same website in Internet Explorer, use the following link:
<pre>
anyapp://ie:https@example.com:8042/over/there/index.dtb?type=animal
</pre>



