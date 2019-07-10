export GOOS=linux
export GOARCH=amd64

go build -o main main.go

# FYI, only works if this tool is on your path
build-lambda-zip -o main.zip main