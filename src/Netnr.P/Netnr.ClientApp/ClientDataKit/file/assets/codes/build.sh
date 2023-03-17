SCRIPTPATH="$( cd -- "$(dirname "$0")" >/dev/null 2>&1 ; pwd -P )"
ls -l $SCRIPTPATH/*.jsx | grep -o '[^/]*$' > $SCRIPTPATH/list.txt