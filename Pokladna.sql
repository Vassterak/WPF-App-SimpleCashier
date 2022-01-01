CREATE TABLE `Products` (
  `id` varchar(255) PRIMARY KEY,
  `title` varchar(255),
  `price` float,
  `quantity` int,
  `isAvaible` bool
);

CREATE TABLE `HistoryOfPurchase` (
  `order_id` varchar(255),
  `product_id` varchar(255),
  `title` varchar(255),
  `price` float,
  `quantity` int,
  PRIMARY KEY (`order_id`, `product_id`)
);

ALTER TABLE `HistoryOfPurchase` ADD FOREIGN KEY (`product_id`) REFERENCES `Products` (`id`);

