# Windows Force Delete

A helpful Windows app that allows you to force delete files and folders that are locked by other processes.

> It also allows you to kill the processes holding the files/folders.

## Installation

1. Download the latest release from the [releases](https://github.com/theminji/windows-force-delete/releases) page.
2. Run the installer to install the context menu.

## Usage

1. Right-click on the file or folder you want to delete. (May have to click "Show more options" first)
2. Click on "Force Delete".
3. The application will open and show you the processes that are locking the file or folder.
4. Click on "Kill Locking Tasks" to kill the processes. Or, click on "Force Delete" to delete the file or folder.

## Test it

You can test it simply by running `python lock_file.py` and it will lock a file (test.txt) so that you can test the app.