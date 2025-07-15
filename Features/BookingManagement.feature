Feature: Booking Management
  In order to manage booking records
  As an API user
  I want to create, retrieve, update, and delete bookings

  Background:
    Given the API is healthy
    And I have a valid authentication token

  @BookingManagement @Functional
  Scenario: Create a new booking
    Given I have valid booking details
    When I create a booking
    Then the API should return a new booking ID
    And the booking ID should be greater than 0

  @BookingManagement @Functional
  Scenario: Retrieve an existing booking
    Given I have created a booking with valid details
    When I retrieve the booking by its ID
    Then the booking details should match the original booking

  @BookingManagement @Functional
  Scenario: Update an existing booking
    Given I have created a booking with valid details
    And I have updated booking details
    When I update the booking with new details
    Then the API should confirm the update was successful
    And retrieving the booking should return the updated details

  @BookingManagement @Functional
  Scenario: Delete an existing booking
    Given I have created a booking with valid details
    When I delete the booking by its ID
    Then the API should confirm the booking was deleted
    And retrieving the booking should return no result