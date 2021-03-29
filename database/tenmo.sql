USE master
GO

IF DB_ID('tenmo') IS NOT NULL
BEGIN
	ALTER DATABASE tenmo SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
	DROP DATABASE tenmo;
END

CREATE DATABASE tenmo
GO

USE tenmo
GO

CREATE TABLE transfer_types (
	transfer_type_id int IDENTITY(1,1) NOT NULL,
	transfer_type_desc varchar(10) NOT NULL,
	CONSTRAINT PK_transfer_types PRIMARY KEY (transfer_type_id)
)

CREATE TABLE transfer_statuses (
	transfer_status_id int IDENTITY(1,1) NOT NULL,
	transfer_status_desc varchar(10) NOT NULL,
	CONSTRAINT PK_transfer_statuses PRIMARY KEY (transfer_status_id)
)

CREATE TABLE users (
	user_id int IDENTITY(1001,1) NOT NULL,
	username varchar(50) NOT NULL,
	password_hash varchar(200) NOT NULL,
	salt varchar(200) NOT NULL,
	CONSTRAINT PK_user PRIMARY KEY (user_id),
	CONSTRAINT UQ_username UNIQUE (username)
)

CREATE TABLE accounts (
	account_id int IDENTITY(2001,1) NOT NULL,
	user_id int NOT NULL,
	balance decimal(13, 2) NOT NULL,
	CONSTRAINT PK_accounts PRIMARY KEY (account_id),
	CONSTRAINT FK_accounts_user FOREIGN KEY (user_id) REFERENCES users (user_id)
)

CREATE TABLE transfers (
	transfer_id int IDENTITY(3001,1) NOT NULL,
	transfer_type_id int NOT NULL,
	transfer_status_id int NOT NULL,
	account_from int NOT NULL,
	account_to int NOT NULL,
	amount decimal(13, 2) NOT NULL,
	CONSTRAINT PK_transfers PRIMARY KEY (transfer_id),
	CONSTRAINT FK_transfers_accounts_from FOREIGN KEY (account_from) REFERENCES accounts (account_id),
	CONSTRAINT FK_transfers_accounts_to FOREIGN KEY (account_to) REFERENCES accounts (account_id),
	CONSTRAINT FK_transfers_transfer_statuses FOREIGN KEY (transfer_status_id) REFERENCES transfer_statuses (transfer_status_id),
	CONSTRAINT FK_transfers_transfer_types FOREIGN KEY (transfer_type_id) REFERENCES transfer_types (transfer_type_id),
	CONSTRAINT CK_transfers_not_same_account CHECK  ((account_from<>account_to)),
	CONSTRAINT CK_transfers_amount_gt_0 CHECK ((amount>0))
)


INSERT INTO transfer_statuses (transfer_status_desc) VALUES ('Pending');
INSERT INTO transfer_statuses (transfer_status_desc) VALUES ('Approved');
INSERT INTO transfer_statuses (transfer_status_desc) VALUES ('Rejected');

INSERT INTO transfer_types (transfer_type_desc) VALUES ('Request');
INSERT INTO transfer_types (transfer_type_desc) VALUES ('Send');

SELECT *
FROM users
JOIN accounts ON users.user_id = accounts.user_id;

SELECT balance FROM accounts WHERE account_id = 2001;

SELECT account_id FROM accounts JOIN users ON accounts.user_id = users.user_id WHERE users.user_id = 1001;

SELECT transfer_id, username AS to_user, account_to, account_from, amount FROM transfers JOIN accounts ON transfers.account_to = accounts.account_id JOIN users ON accounts.user_id = users.user_id WHERE account_from = 2001;

SELECT transfer_id, u1.username AS from_user, u2.username AS to_user, account_to, account_from, amount
FROM transfers
JOIN accounts ac1 ON transfers.account_from = ac1.account_id
JOIN users u1 ON ac1.user_id = u1.user_id
JOIN accounts ac2 ON transfers.account_to = ac2.account_id
JOIN users u2 ON ac2.user_id = u2.user_id
WHERE account_to = 2001;

SELECT transfer_id, transfer_type_desc, transfer_status_desc, account_from, account_to, amount 
FROM transfers 
JOIN transfer_types ON transfers.transfer_status_id = transfer_types.transfer_type_id
JOIN transfer_statuses ON transfers.transfer_status_id = transfer_statuses.transfer_status_id
WHERE account_to = 2002 OR account_from = 2002;


INSERT INTO transfers (transfer_type_id, transfer_status_id, account_from, account_to, amount)
VALUES (2, 2, 2001, 2002, 100);

UPDATE accounts SET balance = balance + 100
WHERE account_id = 2002;

SELECT account_id FROM accounts WHERE user_id = 1001;



select * from transfers;
