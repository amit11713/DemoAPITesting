Feature: Parallel Execution Verification
  In order to ensure the test suite performs efficiently
  As a test automation engineer
  I want to verify that Reqnroll scenarios can execute in parallel

  @ParallelExecution @Unit
  Scenario: Verify parallel execution scenario 1
    When I execute a parallel test with identifier "scenario1"
    Then the test should complete successfully
    And the test should log the correct thread information

  @ParallelExecution @Unit
  Scenario: Verify parallel execution scenario 2
    When I execute a parallel test with identifier "scenario2"
    Then the test should complete successfully
    And the test should log the correct thread information

  @ParallelExecution @Unit
  Scenario: Verify parallel execution scenario 3
    When I execute a parallel test with identifier "scenario3"
    Then the test should complete successfully
    And the test should log the correct thread information

  @ParallelExecution @Unit
  Scenario: Verify parallel execution scenario 4
    When I execute a parallel test with identifier "scenario4"
    Then the test should complete successfully
    And the test should log the correct thread information