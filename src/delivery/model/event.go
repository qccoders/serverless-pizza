package model

type Event struct {
	Type  string `dynamodbav:"type"`
	Start string `dynamodbav:"start"`
	End   int    `dynamodbav:"end"`
}
