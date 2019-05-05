#!/bin/bash

mkdir tmp

sheets=""
for i in {0..3}
do
	sheets="$sheets raw/sheet-$i.png"
done 
montage $sheets -tile 1x -geometry 128x32+0+0 "tmp/tmp-sheet-0.png"
sheets=""
for i in {4..7}
do
	sheets="$sheets raw/sheet-$i.png"
done 
montage $sheets -tile 1x -geometry 128x32+0+0 "tmp/tmp-sheet-1.png"

montage tmp/tmp-sheet-0.png tmp/tmp-sheet-1.png -tile x1 -geometry 128x128+10+0 tmp/sheet-example.png

convert tmp/sheet-example.png -resize 512x final/sheet-example.png

rm -r tmp