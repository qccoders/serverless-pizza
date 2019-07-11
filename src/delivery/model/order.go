package model

type Order struct {
	ID      string  `dynamodbav:"id"`
	Name    string  `dynamodbav:"name"`
	Placed  string  `dynamodbav:"placed"`
	Events  []Event `dynamodbav:"events"`
	Details Details `dynamodbav:"details"`
}
