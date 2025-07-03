Feature: Booking Management
  As an API consumer
  I want to create, retrieve, update, and delete bookings
  So that I can manage booking records in the system

  Background:
    Given the API is healthy
    And I have a valid authentication token

  Scenario: Create a new booking
    When I create a booking with valid details
    Then the API should return a new booking ID
    And the booking details should match the input

  Scenario: Retrieve an existing booking
    Given a booking has been created
    When I retrieve the booking by its ID
    Then the booking details should match the original booking

  Scenario: Update an existing booking
    Given a booking has been created
    When I update the booking with new details
    Then the API should confirm the update was successful
    And retrieving the booking should return the updated details

  Scenario: Delete an existing booking
    Given a booking has been created
    When I delete the booking by its ID
    Then the API should confirm the booking was deleted
    And retrieving the booking should return no result