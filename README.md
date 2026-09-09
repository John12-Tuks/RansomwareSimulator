# Ransomware Simulator

## What It Does

This project is an educational ransomware simulator that demonstrates a simplified ransomware payment workflow.

The simulator:

1. Encrypts designated dummy/test files.
2. Initiates a simulated ransom payment process.
3. Uses Paystack or Stripe **test/sandbox APIs** to process a dummy payment.
4. Verifies the test payment.
5. Decrypts the files after successful payment verification.
6. Closes the simulation once the files have been restored.

No real ransom payments are processed.

## Safety Scope

This project is strictly intended for **educational and authorized cybersecurity testing**.

* Only dummy/test files should be used.
* Payment integrations must remain in test/sandbox mode.
* It should only be run in an isolated lab, virtual machine, or authorized environment.
* It must not be used against production systems, third-party systems, or data without explicit authorization.
* Real payment credentials and real ransom payments must not be used.
