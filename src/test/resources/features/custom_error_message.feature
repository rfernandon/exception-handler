Feature: Display custom messages when an error occurs
  As a user
  I want to receive friendly messages
  when an error occurs

  @CustomErrorApp
  Scenario Outline: Should display the customized error message
    Given the endpoint "/simulation" with path "<code>" and param "<param>"
    When the customer calls the endpoint using method GET
    Then the client receives status http of <status>
    And the client receives server message "<message>" and code "<code>"

    Examples:
      | param               | status | code | message                           |
      | CallErrorException  | 500    | 01   | Test message: CallErrorException  |
      | BadRequestException | 400    | 01   | Test message: BadRequestException |

  @CustomErrorApp
  Scenario: Should display the customized error message with the validation rules
    Given the endpoint "/simulation"
    And the class "TestModel" in package "com.rfernandon.exceptionhandler.support.model"
    And with value "1234" in field "setName"
    When the customer calls the endpoint using method POST
    Then the client receives status http of 422
    And the client receives server code "422" with the validation messages:
      | size: must not be null               |
      | name: size must be between 10 and 60 |

  @CustomErrorSpring
  Scenario: Should display the customized message when not finding the resource
    Given the endpoint "/notfound"
    When the customer calls the endpoint using method GET
    Then the client receives status http of 404
    And the client receives server message "Not Found [No message available]" and code "404"

  @CustomErrorSpring
  Scenario: Should display the generic error message when an unhandled error occurs
    Given the endpoint "/internalerrorserver"
    When the customer calls the endpoint using method GET
    Then the client receives status http of 500
    And the client receives server message "String index out of range: -10" and code "500"
