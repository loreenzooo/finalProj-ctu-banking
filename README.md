# Simple Banking WebForms Application

A web-based banking simulation system featuring user authentication, account management, and transaction processing with specific business logic constraints.

## 📋 Project Overview
This project implements a secure banking portal where users can register, manage their accounts, and perform financial transactions (Deposits and Withdrawals) under strictly defined banking rules.

## 🚀 Key Features

### 1. User Authentication & Management
* **User Registration:** Secure form to create a new banking profile.
* **User Login:** Authenticated access to the banking system.
* **User Logout:** Secure session termination.
* **Change Password:** Functionality to update account security credentials.

### 2. User Dashboard
Displays real-time account information for the currently signed-in user:
* **Account Number:** Unique system-generated ID.
* **Full Name:** User's registered name.
* **Date Registered:** The timestamp of account creation.
* **Current Balance:** Live view of available funds.

### 3. Banking Transactions (Business Logic)

#### **Deposit System**
* **Min/Max:** Minimum of 100.00 and Maximum of 2,000.00 per transaction.
* **Denomination:** Amounts must be divisible by 100.00.
* **Account Ceiling:** A user's total balance cannot exceed 10,000.00.

#### **Withdrawal System**
* **Pre-check:** Displays the current balance before the transaction starts.
* **Min/Max:** Minimum of 100.00 and Maximum of 2,000.00 per transaction.
* **Denomination:** Amounts must be divisible by 100.00.
* **Validation:** Prevents withdrawal if funds are insufficient.
