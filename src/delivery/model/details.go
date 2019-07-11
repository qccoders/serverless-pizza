package model

type Details struct {
	Size     string   `dynamodbav:"size"`
	Type     string   `dynamodbav:"type"`
	CookTime int      `dynamodbav:"cookTime"`
	Toppings []string `dynamodbav:"toppings"`
}
