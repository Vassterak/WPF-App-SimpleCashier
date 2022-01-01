CREATE TABLE `Products` (
  `id` varchar(255) PRIMARY KEY,
  `title` varchar(255),
  `price` float,
  `quantity` int,
  `isAvaible` bool
);

CREATE TABLE `HistoryOfPurchases` (
  `order_id` varchar(255),
  `product_id` varchar(255),
  `title` varchar(255),
  `price` float,
  `quantity` int,
  `timeOfPurchase` date,
  PRIMARY KEY (`order_id`, `product_id`)
);

ALTER TABLE `HistoryOfPurchases` ADD FOREIGN KEY (`product_id`) REFERENCES `Products` (`id`);

