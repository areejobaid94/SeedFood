SET IDENTITY_INSERT [dbo].[LocationLevels] ON 
GO
INSERT [dbo].[LocationLevels] ([Id], [LevelName]) VALUES (1, N'Cities')
GO
INSERT [dbo].[LocationLevels] ([Id], [LevelName]) VALUES (2, N'Areas')
GO
INSERT [dbo].[LocationLevels] ([Id], [LevelName]) VALUES (3, N'District')
GO
SET IDENTITY_INSERT [dbo].[LocationLevels] OFF
GO
SET IDENTITY_INSERT [dbo].[Locations] ON 
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (1, N'عمان', 1, NULL, NULL, N'Amman')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (2, N'الزرقاء', 1, NULL, NULL, N'Zarqa')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (3, N'اربد', 1, NULL, NULL, N'Irbid')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (4, N'مادبا', 1, NULL, NULL, N'Madaba')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (5, N'السلط', 1, NULL, NULL, N'Salat')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (6, N'العقبة', 1, NULL, NULL, N'Aqaba')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (7, N'شفا بدران
', 2, 1, N'https://www.google.com/maps/place/Shafa+Badran,+Amman/@32.0540988,35.8643363,12z/data=!3m1!4b1!4m5!3m4!1s0x151b61f30a3ec9b3:0xd75cd82fef1a8494!8m2!3d32.0538475!4d35.9114323?hl=en
', N'Shafa Badran
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (8, N'أبو نصير
', 2, 1, N'https://www.google.com/maps/place/Abu+Nseir,+Amman/@32.0654904,35.8643391,14z/data=!3m1!4b1!4m5!3m4!1s0x151c9e6579ff6a5f:0x6d5493b986854c66!8m2!3d32.0625049!4d35.8844281', N'Abu Nseir
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (9, N'الجبيهة
', 2, 1, NULL, N'Al Jubeiha
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (10, N'طارق
', 2, 1, NULL, N'Tareq
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (11, N'صويلح
', 2, 1, NULL, N'Sweileh
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (12, N'تلاع العلي أم السماق خلدا
', 2, 1, NULL, N'Tla''a Al Ali & Um Al Summaq & Khalda
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (13, N'العبدلي
', 2, 1, NULL, N'Al Abdali
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (14, N'بسمان
', 2, 1, NULL, N'Basman
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (15, N'ماركا
', 2, 1, NULL, N'Marka
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (16, N'النصر
', 2, 1, NULL, NULL)
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (17, N'المدينة
', 2, 1, NULL, N'Al Madinah
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (18, N'اليرموك
', 2, 1, NULL, N'Al Yarmouk
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (19, N'راس العين
', 2, 1, NULL, N'Ras al-Ain
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (20, N'بدر
', 2, 1, NULL, N'Badr
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (21, N'وادي السير
', 2, 1, NULL, N'Wadi As-Seir
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (22, N'بدر الجديدة
', 2, 1, NULL, NULL)
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (23, N'زهران
', 2, 1, NULL, NULL)
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (24, N'ام قصير والمقابلين
', 2, 1, NULL, N'Al Muqabalayn')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (25, N'خربة السوق، جاوا ، اليادودة
', 2, 1, NULL, N'Al Yadudah')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (26, N'الجيزة 
', 2, 1, NULL, N'Al Jizah
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (27, N'الموقر
', 2, 1, NULL, N'Al-Muwaqqar')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (28, N'القويسمة أبو علندا الرجيب
', 2, 1, NULL, N'Al Quwaysimah
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (29, N'الزرقاء', 2, 2, NULL, N'Zarqa')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (30, N'اربد', 2, 3, NULL, N'Irbid')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (31, N'مادبا', 2, 4, NULL, N'Madaba')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (32, N'السلط', 2, 5, NULL, N'Salat')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (33, N'العقبة', 2, 6, NULL, N'Aqaba')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (34, N'أبو القرام
', 3, 7, N'https://www.google.com/maps/place/Abu+Al+Qram,+Amman/@32.0630789,35.8980764,15z/data=!3m1!4b1!4m5!3m4!1s0x151c9e05b5a57d75:0x78b6c8f63d337fb8!8m2!3d32.0633721!4d35.9062645
', N'Abu Al Qram
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (35, N'الامير حمزة
', 3, 7, N'https://www.google.com/maps/place/Al+Amir+Hamzah,+Amman/@32.0347065,35.913675,14z/data=!3m1!4b1!4m5!3m4!1s0x151b61c42ed68609:0x5aded619104d3c1d!8m2!3d32.0335454!4d35.9259325
', N'Al Amir Hamzah
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (36, N'القصبات', 3, 7, N'https://www.google.com/maps/place/Al+Qasabat,+Amman/@32.0353336,35.8982169,15z/data=!3m1!4b1!4m5!3m4!1s0x151c9e2e7d4c7863:0x6093131d420e59ec!8m2!3d32.0359992!4d35.9020573', N'Al Qasabat')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (37, N'الكوم الشرقي', 3, 7, N'https://www.google.com/maps/place/Al+Kom+Al+Sharqi,+Amman/@32.0368549,35.95005,14z/data=!3m1!4b1!4m5!3m4!1s0x151b6171ae13e0f7:0x27776dc59e1f5fb5!8m2!3d32.0324997!4d35.9735254
', N'Al Kom Al Sharqi
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (38, N'الكوم الغربي
', 3, 7, N'https://www.google.com/maps/place/Al+Kom+Al+Gharbi,+Amman/@32.036982,35.9347291,15z/data=!3m1!4b1!4m5!3m4!1s0x151b61bdede6acbf:0xe9bd8e9ab817fbf9!8m2!3d32.0390903!4d35.9414391?hl=en
', N'Al Kom Al Gharbi
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (39, N'المروج
', 3, 7, N'https://www.google.com/maps/place/Al+Morouj,+Amman/@32.0501551,35.9137516,15z/data=!3m1!4b1!4m5!3m4!1s0x151b61e43b1c0793:0x75346e606479ebe7!8m2!3d32.0504901!4d35.9237938?hl=en
', N'Al Morouj
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (40, N'الموحدين
', 3, 7, N'https://www.google.com/maps/place/Al+Mowahdin,+Amman/@32.0684952,35.8913084,15z/data=!3m1!4b1!4m5!3m4!1s0x151c9e096d7f0429:0x2530be73ccef775b!8m2!3d32.0685401!4d35.897704?hl=en
', N'Al Mowahdin
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (41, N'أم العروق
', 3, 7, N'https://www.google.com/maps/place/Umm+Al+Irouq,+Amman/@32.0783877,35.9098576,14z/data=!3m1!4b1!4m5!3m4!1s0x151b62121641d5b1:0xd145e8eb4d2d8867!8m2!3d32.0781769!4d35.925574?hl=en
', N'Umm Al Irouq
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (42, N'ام بلانة
', 3, 7, N'https://www.google.com/maps/place/Umm+Blaneh,+Amman/@32.0722329,35.9068866,15z/data=!3m1!4b1!4m5!3m4!1s0x151b62075a4ad241:0x732b709a109234aa!8m2!3d32.0717686!4d35.9163965?hl=en
', N'Umm Blaneh
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (43, N'ام حجير
', 3, 7, N'https://www.google.com/maps/place/Umm+Hjair,+Amman/@32.0722329,35.9068866,15z/data=!4m5!3m4!1s0x151c9e48e05c1557:0x6fcdc35c29d51e00!8m2!3d32.045944!4d35.886917?hl=en
', N'Umm Hjair
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (44, N'أم شطيرات
', 3, 7, N'https://www.google.com/maps/place/Umm+Shterat,+Amman/@32.0827995,35.8727453,13z/data=!3m1!4b1!4m5!3m4!1s0x151c9df16d4295db:0xbb69011b541b6f6b!8m2!3d32.0815102!4d35.9035826?hl=en
', N'Umm Shterat
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (45, N'ذهيبة
', 3, 7, N'https://www.google.com/maps/place/Dhuheibah,+Amman/@32.0519085,35.8857191,14z/data=!3m1!4b1!4m5!3m4!1s0x151c9e17eb76bb25:0xcae5af92607d93c2!8m2!3d32.0488839!4d35.9038682?hl=en
', N'Dhuheibah
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (46, N'طاب كراع
', 3, 7, N'https://www.google.com/maps/place/Tab+Qira,+Amman/@32.0516932,35.875531,14z/data=!3m1!4b1!4m5!3m4!1s0x151c9e407dfad64b:0x84f8f46984bce432!8m2!3d32.0513367!4d35.8926052?hl=en
', N'Tab Qira
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (47, N'عيون الذيب
', 3, 7, N'https://www.google.com/maps/place/Uyun+Al+Dhib,+Amman/@32.0676072,35.9068391,14z/data=!3m1!4b1!4m5!3m4!1s0x151b62036c6fadcb:0x87e19ff191df95b7!8m2!3d32.0610447!4d35.9164089?hl=en
', N'Uyun Al Dhib
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (48, N'مرج الفرس
', 3, 7, N'https://www.google.com/maps/place/Marj+Al+Furs,+Amman/@32.056204,35.9116107,13z/data=!3m1!4b1!4m5!3m4!1s0x151b61881f3985f3:0xf79d6efcb3d89757!8m2!3d32.045545!4d35.9365548?hl=en
', N'Marj Al Furs
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (49, N'ياجوز
', 3, 7, N'https://www.google.com/maps/place/Yajouz,+Amman/@32.0355213,35.8996437,14z/data=!3m1!4b1!4m5!3m4!1s0x151b61d790c6d4a9:0x3a2c54a181b398d8!8m2!3d32.0391553!4d35.9135363?hl=en
', N'Yajouz
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (50, N'الامانة
', 3, 8, N'https://www.google.com/maps/place/Al+Amanah,+Amman/@32.0597756,35.8565368,14z/data=!3m1!4b1!4m5!3m4!1s0x151c9e5fd23d17ef:0xdc0eb1534940a25c!8m2!3d32.056807!4d35.8708057?hl=en
', N'Al Amanah
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (51, N'البسالة
', 3, 8, N'https://www.google.com/maps/place/Al+Basalah,+Amman/@32.0550758,35.8705095,15z/data=!3m1!4b1!4m5!3m4!1s0x151c9e5c5cf9c68d:0x9c26e9933cd9f3ca!8m2!3d32.055115!4d35.8791426?hl=en
', N'Al Basalah
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (52, N'السعادة
', 3, 8, N'https://www.google.com/maps/place/Al+Sa''adah,+Amman/@32.0645628,35.8720743,15z/data=!3m1!4b1!4m5!3m4!1s0x151c9e65beabcc37:0x142f2de5f2d3b304!8m2!3d32.0650362!4d35.8815725?hl=en
', N'Al Sa''adah
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (53, N'الضياء
', 3, 8, N'https://www.google.com/maps/place/Al+Dia'',+Amman/@32.0738529,35.868748,14z/data=!3m1!4b1!4m5!3m4!1s0x151c9e765934b90d:0xc1f9a2a79d7a18c4!8m2!3d32.0736797!4d35.8858495?hl=en
', N'Al Dia''
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (54, N'الفاروق
', 3, 8, N'https://www.google.com/maps/place/Al+Farouq,+Amman/@32.0594053,35.8792608,15z/data=!3m1!4b1!4m5!3m4!1s0x151c9e6bdfee5311:0xa64366dca62a9ba5!8m2!3d32.056912!4d35.8861039?hl=en
', N'Al Farouq')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (55, N'المحبة
', 3, 8, N'https://www.google.com/maps/place/Al+Mahabbah,+Amman/@32.0484251,35.8706691,15z/data=!3m1!4b1!4m5!3m4!1s0x151c9e5083ddf1c9:0x891bddef9ce865b1!8m2!3d32.0485647!4d35.8786284?hl=en
', N'Al Mahabbah
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (56, N'ابن عوف
', 3, 9, N'https://www.google.com/maps/place/Ibn+Owf,+Amman/@32.0359083,35.8439602,14z/data=!3m1!4b1!4m5!3m4!1s0x151c9f0547eb7ae9:0xeac1bcc620777fd3!8m2!3d32.0335178!4d35.859938?hl=en
', N'Ibn Owf
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (57, N'البلدية
', 3, 9, N'https://www.google.com/maps/place/Al+Baladiyah,+Amman/@32.0244843,35.8486828,14z/data=!3m1!4b1!4m5!3m4!1s0x151c9f0b5ff3d7c3:0x71d030532d6aefde!8m2!3d32.0244306!4d35.8650511?hl=en
', N'Al Baladiyah
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (58, N'الجامعة
', 3, 9, N'https://www.google.com/maps/place/Al+Jama''a,+Amman/@32.0187938,35.8819448,3a,92.3y,90t/data=!3m8!1e2!3m6!1sAF1QipNXBs3-enXiSvD4xIjAqdTNuvJm8xO-y6qshnsf!2e10!3e12!6shttps:%2F%2Flh5.googleusercontent.com%2Fp%2FAF1QipNXBs3-enXiSvD4xIjAqdTNuvJm8xO-y6qshnsf%3Dw128-h86-k-no!7i512!8i342!4m5!3m4!1s0x151c9f9ed5db86ed:0xda30eb1d7d001735!8m2!3d32.0187938!4d35.8819448?hl=en
', N'Al Jama''a
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (59, N'الريان
', 3, 9, N'https://www.google.com/maps/place/Al+Rayyan,+Amman/@32.038302,35.8563228,14z/data=!3m1!4b1!4m5!3m4!1s0x151c9faaf900b6fd:0x518a10c42e730fcd!8m2!3d32.0337183!4d35.8694264?hl=en
', N'Al Rayyan
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (60, N'الزيتونة
', 3, 9, N'https://www.google.com/maps/place/Al+Zaytonah,+Amman/@32.0346956,35.8776796,14z/data=!3m1!4b1!4m5!3m4!1s0x151c9e353c39d16d:0x64d019def23b97ba!8m2!3d32.0338954!4d35.887871?hl=en
', N'Al Zaytonah
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (61, N'الصديق
', 3, 9, N'https://www.google.com/maps/place/Al+Sadiq,+Amman/@32.0089316,35.8623496,13z/data=!3m1!4b1!4m5!3m4!1s0x151c9feb928de277:0xecc959934ea69b6c!8m2!3d32.0119349!4d35.888024?hl=en
', N'Al Sadiq
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (62, N'الكلية الاسلامية
', 3, 9, N'https://www.google.com/maps/place/Al+Kuliyah+Al+Islamiyah,+Amman/@32.0199447,35.86448,13z/data=!3m1!4b1!4m5!3m4!1s0x151c9fc5d2b1fbe9:0x579f638c966ee4f1!8m2!3d32.0232754!4d35.888882?hl=en
', N'Al Kuliyah Al Islamiyah
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (63, N'المنصور
', 3, 9, N'https://www.google.com/maps/place/Al+Mansour,+Amman/@32.017581,35.8720885,13z/data=!3m1!4b1!4m5!3m4!1s0x151c9fda557384e3:0xd64e0e21a0a2d720!8m2!3d32.0232562!4d35.8972738?hl=en
', N'Al Mansour
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (64, N'قطنة
', 3, 9, N'https://www.google.com/maps/place/Qutnah,+Amman/@31.9998627,35.8922784,14z/data=!3m1!4b1!4m5!3m4!1s0x151c9fe358024f79:0xf9591a1c0d8e8fe9!8m2!3d31.9942835!4d35.9089072?hl=en
', N'Qutnah
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (65, N'ابو عليا
', 3, 10, N'https://www.google.com/maps/place/Abu+Alia,+Amman/@32.0062025,35.9272537,13z/data=!3m1!4b1!4m5!3m4!1s0x151b60f93a951ab7:0xb6b092bd6e23372!8m2!3d32.0041059!4d35.9590599?hl=en
', N'Abu Alia
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (66, N'الأمير الحسين بن عبدالله
', 3, 10, N'https://www.google.com/maps/place/Al+Amir+Al+Hussein+Ben+Abdullah,+Amman/@32.0151093,35.9110257,14z/data=!3m1!4b1!4m5!3m4!1s0x151b6030541a9ac1:0x6ac2d11a77ccc1c6!8m2!3d32.0199339!4d35.9350135?hl=en
', N'Al Amir Al Hussein Ben Abdullah
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (67, N'الخزنة
', 3, 10, N'https://www.google.com/maps/place/Al+Khaznah,+Amman/@32.0013471,35.9090969,14z/data=!3m1!4b1!4m5!3m4!1s0x151b601783aa4117:0x5834a5e60e6be5d4!8m2!3d31.9998971!4d35.9247583?hl=en
', N'Al Khaznah
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (68, N'الشهيد الجنوبي
', 3, 10, N'https://www.google.com/maps/place/Al+Shahid+Al+Janobi,+Amman/@31.9856523,35.9290958,13z/data=!3m1!4b1!4m5!3m4!1s0x151b60894c9407e3:0x3ce7edd75c17f94a!8m2!3d31.9860838!4d35.960306?hl=en
', N'Al Shahid Al Janobi
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (69, N'الشهيد الشمالي
', 3, 10, N'https://www.google.com/maps/place/Al+Shahid+Al+Shamali,+Amman/@31.9945634,35.9277198,13z/data=!3m1!4b1!4m5!3m4!1s0x151b60f45e4e48d7:0x586b860e5f94d245!8m2!3d31.9919873!4d35.9561884?hl=en
', N'Al Shahid Al Shamali
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (70, N'الغابة
', 3, 10, N'https://www.google.com/maps/place/Al+Ghaba,+Amman/@32.0171697,35.9175242,13z/data=!3m1!4b1!4m5!3m4!1s0x151b61ac88ccaf51:0xc3901cc671749498!8m2!3d32.0144433!4d35.9522293?hl=en
', N'Al Ghaba
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (71, N'الفيصل
', 3, 10, N'https://www.google.com/maps/place/Al+Feisal,+Amman/@32.0153919,35.954607,14z/data=!3m1!4b1!4m5!3m4!1s0x151b611b061e5d79:0x3469da230594f326!8m2!3d32.0108919!4d35.9701159?hl=en
', N'Al Feisal
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (72, N'طبربور
', 3, 10, N'https://www.google.com/maps/place/Tabarbour,+Amman/@32.0014116,35.9236947,14z/data=!3m1!4b1!4m5!3m4!1s0x151b6041756fc2ef:0x2cb3357765e57e7c!8m2!3d32.0028488!4d35.9403639?hl=en
', N'Tabarbour
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (73, N'عين رباط
', 3, 10, N'https://www.google.com/maps/place/Ayn+Rbat,+Amman/@32.0160516,35.9543158,13z/data=!3m1!4b1!4m5!3m4!1s0x151b613a4f93530b:0xc3d2442b61839f98!8m2!3d32.0073876!4d35.9747874?hl=en
', N'Ayn Rbat
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (74, N'عين غزال
', 3, 10, N'https://www.google.com/maps/place/Ayn+Ghazal,+Amman/@32.0001683,35.9700034,14z/data=!3m1!4b1!4m5!3m4!1s0x151b60d9167f62bf:0x5d81c404ad4c5a22!8m2!3d31.9987934!4d35.9802504?hl=en
', N'Ayn Ghazal
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (75, N'البشائر
', 3, 11, N'https://www.google.com/maps/place/Al+Bsha''er,+Amman/@32.0062008,35.8105614,13z/data=!3m1!4b1!4m5!3m4!1s0x151ca1e1472faba5:0x58512338500a5e87!8m2!3d32.0130575!4d35.8496618?hl=en
', N'Al Bsha''er
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (76, N'الحمر
', 3, 11, N'https://www.google.com/maps/place/Al+Hummar,+Amman/@32.0112812,35.7813598,13z/data=!3m1!4b1!4m5!3m4!1s0x151ca20f92715fdd:0x315cbefa7b68c332!8m2!3d32.007386!4d35.8063475?hl=en
', N'Al Hummar
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (77, N'الحي الشرقي
', 3, 11, N'https://www.google.com/maps/place/Al+Hai+Al+Sharqi,+Amman/@32.0293593,35.8276233,14z/data=!3m1!4b1!4m5!3m4!1s0x151c9f40ba5e8819:0x1bb04782d6337eab!8m2!3d32.0269783!4d35.8477227?hl=en
', N'Al Hai Al Sharqi
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (78, N'الرحمانية
', 3, 11, N'https://www.google.com/maps/place/Al+Rahmanyeh,+Amman/@32.0075499,35.8125339,14z/data=!3m1!4b1!4m5!3m4!1s0x151ca1f85755fd51:0xe1ff015ceab5787e!8m2!3d32.0116044!4d35.8326315?hl=en
', N'Al Rahmanyeh
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (79, N'الفروسية
', 3, 11, N'https://www.google.com/maps/place/Al+Forousyah,+Amman/@32.0214139,35.7762851,13z/data=!3m1!4b1!4m5!3m4!1s0x151c98a0aa1cd3d9:0x59efda5516e738a!8m2!3d32.0264129!4d35.804359?hl=en
', N'Al Forousyah
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (80, N'الفضيلة
', 3, 11, N'https://www.google.com/maps/place/Al+Fadilah,+Amman/@32.0202178,35.8379415,14z/data=!3m1!4b1!4m5!3m4!1s0x151c9f6ecc5e8041:0x11f9677609664136!8m2!3d32.0198692!4d35.8541239?hl=en
', N'Al Fadilah
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (81, N'الكمالية
', 3, 11, N'https://www.google.com/maps/place/Al+Kamalyah,+Amman/@32.0303747,35.7852771,13z/data=!3m1!4b1!4m5!3m4!1s0x151c98b1aef90e0f:0x9570b918ce2a9008!8m2!3d32.0318345!4d35.8227975?hl=en
', N'Al Kamalyah
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (82, N'حدائق الحسين
', 3, 11, N'https://www.google.com/maps/place/Hada''eq+Al+Hussein,+Amman/@31.9874134,35.8079498,14z/data=!3m1!4b1!4m5!3m4!1s0x151ca227d39c9f31:0xf57d53250f12af93!8m2!3d31.9886625!4d35.8239792?hl=en
', N'Hada''eq Al Hussein
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (83, N'دابوق
', 3, 11, N'https://www.google.com/maps/place/Dabouq,+Amman/@31.9860651,35.7920908,14z/data=!3m1!4b1!4m5!3m4!1s0x151ca234381c5775:0xc756498fe233963c!8m2!3d31.9891204!4d35.806491?hl=en
', N'Dabouq
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (84, N'ميسلون
', 3, 11, N'https://www.google.com/maps/place/Maisloun,+Amman/@32.0227311,35.8167776,14z/data=!3m1!4b1!4m5!3m4!1s0x151c9f51edad916d:0x305b48f9c5c8a779!8m2!3d32.0245482!4d35.8334664?hl=en
', N'Maisloun
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (85, N'التلاع الشرقي
', 3, 12, N'https://www.google.com/maps/place/Al+Tla''a+Al+Sharqi,+Amman/@31.9836999,35.8444728,13z/data=!3m1!4b1!4m5!3m4!1s0x151ca036d9ed573b:0x532c1946feb4867!8m2!3d31.9823153!4d35.875538?hl=en
', N'Al Tla''a Al Sharqi
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (86, N'التلاع الشمالي
', 3, 12, N'https://www.google.com/maps/place/Al+Tla''a+Al+Shamali,+Amman/@32.0042739,35.845312,14z/data=!3m1!4b1!4m5!3m4!1s0x151ca1d7d0b74835:0xa73ebc3b51dca6c4!8m2!3d31.9984983!4d35.8653707?hl=en
', N'Al Tla''a Al Shamali
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (87, N'الخالدين
', 3, 12, N'https://www.google.com/maps/place/Al+Khaledin,+Amman/@32.0022847,35.8286648,14z/data=!3m1!4b1!4m5!3m4!1s0x151ca1e65bd90263:0x2013b54a8327e5b!8m2!3d32.0010807!4d35.8480988?hl=en
', N'Al Khaledin
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (88, N'السلام
', 3, 12, N'https://www.google.com/maps/place/Al+Salam,+Amman/@31.9726482,35.858912,14z/data=!3m1!4b1!4m5!3m4!1s0x151ca04dd1fabdf9:0xbb8a25a432cfa8ca!8m2!3d31.9736843!4d35.8814843?hl=en
', N'Al Salam
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (89, N'الصالحين
', 3, 12, N'https://www.google.com/maps/place/Al+Salehien,+Amman/@31.9858628,35.8403984,14z/data=!3m1!4b1!4m5!3m4!1s0x151ca1b887e98fd1:0x8d69bfc36fe0aa3a!8m2!3d31.9885933!4d35.8548137?hl=en
', N'Al Salehien
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (90, N'أم السماق
', 3, 12, N'https://www.google.com/maps/place/Umm+Al+Summaq,+Amman/@31.9794835,35.824714,14z/data=!3m1!4b1!4m5!3m4!1s0x151ca19a518cd1a3:0x4a79abd6e3cc1def!8m2!3d31.9816856!4d35.8434314?hl=en
', N'Umm Al Summaq
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (91, N'بركة
', 3, 12, N'https://www.google.com/maps/place/Baraka,+Amman/@31.9927239,35.8481329,13z/data=!3m1!4b1!4m5!3m4!1s0x151ca025fb4620e3:0x201c6c8a4ba9e3d2!8m2!3d31.987854!4d35.8846901?hl=en
', N'Baraka
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (92, N'خلدا
', 3, 12, N'https://www.google.com/maps/place/Khilda,+Amman/@31.9894578,35.8222815,14z/data=!3m1!4b1!4m5!3m4!1s0x151ca19264f539f3:0x2b620355c1fddf92!8m2!3d31.9901273!4d35.8451072?hl=en
', N'Khilda
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (93, N'الشميساني
', 3, 13, N'https://www.google.com/maps/place/Ash+Shumaysani,+Amman/@31.9728949,35.8804685,14z/data=!3m1!4b1!4m5!3m4!1s0x151ca06b4f0fa949:0x767658b1f5642bb5!8m2!3d31.9690675!4d35.899243?hl=en
', N'Ash Shumaysani
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (94, N'جبل الحسين
', 3, 13, N'https://www.google.com/maps/place/Jabal+Al+Hussein,+Amman/@31.96837,35.9032094,14z/data=!3m1!4b1!4m5!3m4!1s0x151b5ff37fc60a8b:0x51690abcb65ef5b4!8m2!3d31.9674446!4d35.9177805?hl=en
', N'Jabal Al Hussein
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (95, N'جبل اللويبدة
', 3, 13, N'https://www.google.com/maps/place/Jabal+Al+Lweibdeh,+Amman/@31.9628302,35.8796093,13z/data=!3m1!4b1!4m5!3m4!1s0x151ca076066bea2b:0xdf97694e38e0b2b2!8m2!3d31.9558443!4d35.9279374?hl=en
', N'Jabal Al Lweibdeh
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (96, N'مدينة الحسين للشباب
', 3, 13, N'https://www.google.com/maps/place/Madinat+Al+Hussein,+Amman/@31.9830794,35.8906292,14z/data=!3m1!4b1!4m5!3m4!1s0x151ca00448a0164d:0xb33c387b967a2318!8m2!3d31.9789356!4d35.9107786?hl=en
', N'Madinat Al Hussein
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (97, N'الجرن
', 3, 14, N'https://www.google.com/maps/place/Al+Jurn,+Amman/@31.9836824,35.9088429,14z/data=!3m1!4b1!4m5!3m4!1s0x151b5fe2ab8d423d:0xefa595effd045d3e!8m2!3d31.9876141!4d35.9239213?hl=en
', N'Al Jurn
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (98, N'الرواق
', 3, 14, N'https://www.google.com/maps/place/Al+Rowaq,+Amman/@31.9825632,35.9192605,14z/data=!3m1!4b1!4m5!3m4!1s0x151b600ad24c563d:0xf7cecb994e74c0b1!8m2!3d31.98403!4d35.9354557?hl=en
', N'Al Rowaq
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (99, N'القصور
', 3, 14, N'https://www.google.com/maps/place/Al+Qusour,+Amman/@31.9641378,35.925043,14z/data=!3m1!4b1!4m5!3m4!1s0x151b5fc400cc1fa3:0x8ebb491c8cdec715!8m2!3d31.9593584!4d35.9370121?hl=en
', N'Al Qusour
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (100, N'الهاشمي الشمالي
', 3, 14, N'https://www.google.com/maps/place/Al+Hashmi+Al+Shamali,+Amman/@31.9707582,35.9207444,13z/data=!3m1!4b1!4m5!3m4!1s0x151b5fd45e118e5d:0x94b6ffd37de7794d!8m2!3d31.9701589!4d35.9589407?hl=en
', N'Al Hashmi Al Shamali
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (101, N'جبل النزهة
', 3, 14, N'https://www.google.com/maps/place/Jabal+Al+Nuzha,+Amman/@31.9716374,35.9103148,14z/data=!3m1!4b1!4m5!3m4!1s0x151b5fefb0cbfebd:0x42f4f4e3abec2099!8m2!3d31.971444!4d35.9290554?hl=en
', N'Jabal Al Nuzha
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (102, N'رغدان
', 3, 14, N'https://www.google.com/maps/place/Raghadan,+Amman/@31.981846,35.9213643,13z/data=!3m1!4b1!4m5!3m4!1s0x151b607d0135aacd:0xe590a604e22fda9b!8m2!3d31.9801045!4d35.954005?hl=en
', N'Raghadan
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (103, N'التطوير
', 3, 15, N'https://www.google.com/maps/place/Al+Tatwir,+Amman/@31.9929078,35.9812155,12z/data=!3m1!4b1!4m5!3m4!1s0x151b6659ff64bf65:0xca7d4c5d0aeaf22!8m2!3d31.9979498!4d36.0529599?hl=en
', N'Al Tatwir
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (104, N'الزهراء
', 3, 15, N'https://www.google.com/maps/place/Al+Zahra,+Amman/@31.989065,35.9676804,13z/data=!3m1!4b1!4m5!3m4!1s0x151b672d56ad49d5:0x5543b682ac8bd635!8m2!3d31.9807795!4d35.9980114?hl=en
', N'Al Zahra')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (105, N'المشيرفة
', 3, 15, N'https://www.google.com/maps/place/Al+Mshairfeh,+Amman/@31.9997151,35.9767674,14z/data=!3m1!4b1!4m5!3m4!1s0x151b60d64cee7dad:0xd7370387b55612c0!8m2!3d32.0019791!4d35.995587?hl=en
', N'Al Mshairfeh
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (106, N'المطار
', 3, 15, N'https://www.google.com/maps/place/Al+Matar,+Amman/@31.974269,35.9562142,13z/data=!3m1!4b1!4m5!3m4!1s0x151b60ba9e82fad3:0x77a3ac0def39089f!8m2!3d31.9771347!4d35.9848947?hl=en
', N'Al Matar
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (107, N'حمزة
', 3, 15, N'https://www.google.com/maps/place/Hamza,+Amman/@31.9797883,35.9465173,13z/data=!3m1!4b1!4m5!3m4!1s0x151b60bdec7ae889:0x6d8d41b06c896cf!8m2!3d31.9876541!4d35.9896359?hl=en
', N'Hamza
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (108, N'الأمير الحسن
', 3, 16, N'https://www.google.com/maps/place/Al+Amir+Hasan,+Amman/@31.9592411,35.9590748,14z/data=!3m1!4b1!4m5!3m4!1s0x151b5e23149db1cd:0x4c7db98209885597!8m2!3d31.958796!4d35.9727803?hl=en
', N'Al Amir Hasan
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (109, N'الاميرة عالية
', 3, 16, N'https://www.google.com/maps/place/Al+Amira+Alia,+Amman/@31.9419495,35.9472436,13z/data=!3m1!4b1!4m5!3m4!1s0x151b5e6a3bf84dff:0x6ec6fac0e48df646!8m2!3d31.9401975!4d35.9735134?hl=en
', N'Al Amira Alia
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (110, N'الربوة
', 3, 16, N'https://www.google.com/maps/place/Al+Rabwa,+Amman/@31.9489595,35.9554538,13z/data=!3m1!4b1!4m5!3m4!1s0x151b5e11ee132d0f:0xb731c55bef5d777a!8m2!3d31.9495574!4d35.9869735?hl=en
', N'Al Rabwa
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (111, N'الصالحية
', 3, 16, N'https://www.google.com/maps/place/Salihiyat+Al+Abid,+Amman/@31.9524218,35.9661225,13z/data=!3m1!4b1!4m5!3m4!1s0x151b5e07745f0087:0x9f4cf6d518b34d2f!8m2!3d31.9565615!4d35.9911426?hl=en
', N'Salihiyat Al Abid
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (112, N'المنارة
', 3, 16, N'https://www.google.com/maps/place/Al+Manarah,+Amman/@31.9435695,35.9441662,14z/data=!3m1!4b1!4m5!3m4!1s0x151b5e4d9c54e8a1:0x7c985e76694a9fd3!8m2!3d31.9447958!4d35.9619123?hl=en
', N'Al Manarah
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (113, N'جبل النصر
', 3, 16, N'https://www.google.com/maps/place/Jabal+Al+Naser,+Amman/@31.9602977,35.9556228,15z/data=!3m1!4b1!4m5!3m4!1s0x151b5e318afc397f:0x5e2ce50fbd20ccad!8m2!3d31.9605156!4d35.9639208?hl=en
', N'Jabal AI Naser
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (114, N'النصر
', 3, 16, N'https://www.google.com/maps/place/Al+Naser,+Amman/@31.9524681,35.9292984,12z/data=!3m1!4b1!4m5!3m4!1s0x151b5e145c1259ed:0x91cb6a187ec0dcd5!8m2!3d31.9559935!4d35.9968806?hl=en
', N'AI naser
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (115, N'الأمير الحسن
', 3, 17, N'https://www.google.com/maps/place/Al+Amir+Hasan,+Amman/@31.9592411,35.9590748,14z/data=!3m1!4b1!4m5!3m4!1s0x151b5e23149db1cd:0x4c7db98209885597!8m2!3d31.958796!4d35.9727803?hl=en
', N'Al Amir Hasan
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (116, N'الاميرة عالية
', 3, 17, N'https://www.google.com/maps/place/Al+Amira+Alia,+Amman/@31.9419495,35.9472436,13z/data=!3m1!4b1!4m5!3m4!1s0x151b5e6a3bf84dff:0x6ec6fac0e48df646!8m2!3d31.9401975!4d35.9735134?hl=en
', N'Al Amira Alia
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (117, N'الربوة
', 3, 17, N'https://www.google.com/maps/place/Al+Rabwa,+Amman/@31.9489595,35.9554538,13z/data=!3m1!4b1!4m5!3m4!1s0x151b5e11ee132d0f:0xb731c55bef5d777a!8m2!3d31.9495574!4d35.9869735?hl=en
', N'Al Rabwa
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (118, N'الرجوم
', 3, 17, N'https://www.google.com/maps/place/Al+Rjoum,+Amman/@31.9524237,35.9198377,15z/data=!3m1!4b1!4m5!3m4!1s0x151b5f9a8b0e76a3:0x7c37f8e8df6648c1!8m2!3d31.9522138!4d35.9277006?hl=en
', N'Al Rjoum
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (119, N'العدلية
', 3, 17, N'https://www.google.com/maps/place/Al+Adlyeh,+Amman/@31.9566964,35.9177304,15z/data=!3m1!4b1!4m5!3m4!1s0x151b5f902cb2aa9f:0x38cc96e8e2f6679d!8m2!3d31.9587435!4d35.9279293?hl=en
', N'Al Adlyeh
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (120, N'المدرج
', 3, 17, N'https://www.google.com/maps/place/Al+Mudaraj,+Amman/@31.9511135,35.9310554,15z/data=!3m1!4b1!4m5!3m4!1s0x151b5fbcdfde4c87:0x4f78c9bc1e5700c9!8m2!3d31.95143!4d35.9373168?hl=en
', N'Al Mudaraj
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (121, N'المنارة
', 3, 17, N'https://www.google.com/maps/place/Al+Manarah,+Amman/@31.9435695,35.9441662,14z/data=!3m1!4b1!4m5!3m4!1s0x151b5e4d9c54e8a1:0x7c985e76694a9fd3!8m2!3d31.9447958!4d35.9619123?hl=en
', N'Al Manarah
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (122, N'المهاجرين
', 3, 17, N'https://www.google.com/maps/place/Al+Muhajirin,+Amman/@31.9472589,35.9071543,14z/data=!3m1!4b1!4m5!3m4!1s0x151b5f82e08a83a3:0xfa6efe0b0ddf0d1b!8m2!3d31.9463103!4d35.9251545?hl=en
', N'Al Muhajirin
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (123, N'الهاشمي الجنوبي
', 3, 17, N'https://www.google.com/maps/place/Al+Hashmi+Al+Janobi,+Amman/@31.964038,35.9427769,15z/data=!3m1!4b1!4m5!3m4!1s0x151b5fce44c63a59:0x37396496e9cedec3!8m2!3d31.9621738!4d35.9526959?hl=en
', N'Al Hashmi Al Janobi
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (124, N'جبل الجوفة
', 3, 17, N'https://www.google.com/maps/place/Jabal+Al+Jofah,+Amman/@31.9557516,35.9319976,14z/data=!3m1!4b1!4m5!3m4!1s0x151b5fb7c3e80e15:0xba7a8677645be8f4!8m2!3d31.9535927!4d35.9516023?hl=en
', N'Jabal Al Jofah
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (125, N'جبل القلعة
', 3, 17, N'https://www.google.com/maps/place/Jabal+Al+Qala''a,+Amman/@31.9538169,35.9297423,15z/data=!3m1!4b1!4m5!3m4!1s0x151b5fbdbe5ba309:0xac21eb5de5e59c1b!8m2!3d31.9526634!4d35.9354185?hl=en
', N'Jabal Al Qala''a
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (126, N'جبل النصر
', 3, 17, N'https://www.google.com/maps/place/Jabal+Al+Naser,+Amman/@31.9602977,35.9556228,15z/data=!3m1!4b1!4m5!3m4!1s0x151b5e318afc397f:0x5e2ce50fbd20ccad!8m2!3d31.9605156!4d35.9639208?hl=en
', N'Jabal Al Naser
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (127, N'وادي الحدادة
', 3, 17, N'https://www.google.com/maps/place/Wadi+Al+Haddadeh,+Amman/@31.9605708,35.9275672,15z/data=!3m1!4b1!4m5!3m4!1s0x151b5f954e1ccf0b:0x8591cc3dee0dde17!8m2!3d31.9600672!4d35.9331127?hl=en
', N'Wadi Al Haddadeh
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (128, N'وادي السرور
', 3, 17, N'https://www.google.com/maps/place/Wadi+Al+Srour,+Amman/@31.9469264,35.9252168,15z/data=!3m1!4b1!4m5!3m4!1s0x151b5f992b60a7dd:0x710c24a8d38c63cf!8m2!3d31.9468212!4d35.9341161?hl=en
', N'Wadi Al Srour
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (129, N'الاشرفية
', 3, 18, N'https://www.google.com/maps/place/Al+Ashrafyeh,+Amman/@31.9412358,35.9194161,14z/data=!3m1!4b1!4m5!3m4!1s0x151b5f9fff3a750b:0xef436b7057075f7a!8m2!3d31.9406491!4d35.9345231?hl=en
', N'Al Ashrafyeh
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (130, N'العوده
', 3, 18, N'https://www.google.com/maps/place/Al+Awdeh,+Amman/@31.9333093,35.924232,14z/data=!3m1!4b1!4m5!3m4!1s0x151b5f0c1793f109:0x6dc3179d2a0e8adf!8m2!3d31.9324818!4d35.9403025?hl=en
', N'Al Awdeh
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (131, N'جبل التاج
', 3, 18, N'https://www.google.com/maps/place/Jabal+Al+Taj,+Amman/@31.9486559,35.9337389,14z/data=!3m1!4b1!4m5!3m4!1s0x151b5fb1a81f7da5:0xaa895ee379c7f4f7!8m2!3d31.9493508!4d35.9502445?hl=en
', N'Jabal Al Taj
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (132, N'الروضة
', 3, 19, N'https://www.google.com/maps/place/Al+Rawda,+Amman/@31.9266796,35.8795917,13z/data=!4m5!3m4!1s0x151b5f5a7a598d41:0xd26e2391a42424a5!8m2!3d31.917066!4d35.921805?hl=en
', N'Al Rawda
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (133, N'الزهور
', 3, 19, N'https://www.google.com/maps/place/Al+Zohour,+Amman/@31.925244,35.9110289,14z/data=!3m1!4b1!4m5!3m4!1s0x151b5f684a2727f1:0x2032995d5ef44680!8m2!3d31.9255386!4d35.9318899?hl=en
', N'Al Zohour
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (134, N'النظيف
', 3, 19, N'https://www.google.com/maps/place/Al+Nathif,+Amman/@31.9374942,35.9195251,15z/data=!3m1!4b1!4m5!3m4!1s0x151b5f70e7bd882f:0xdad4dec67d0dbd71!8m2!3d31.9360741!4d35.9264011?hl=en
', N'Al Nathif
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (135, N'الاخضر
', 3, 20, N'https://www.google.com/maps/place/Al+Akhdar,+Amman/@31.9361211,35.8937906,14z/data=!3m1!4b1!4m5!3m4!1s0x151ca09c424fd751:0xb0fce8816b1fbe91!8m2!3d31.9355798!4d35.9135446?hl=en
', N'Al Akhdar
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (136, N'الحمرانية
', 3, 20, N'https://www.google.com/maps/place/Al+Humranyah,+Amman/@31.921444,35.8857221,14z/data=!3m1!4b1!4m5!3m4!1s0x151ca0b013b697bf:0xf7785e89048c75b!8m2!3d31.9191047!4d35.9017792?hl=en
', N'Al Humranyah
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (137, N'الذراع
', 3, 20, N'https://www.google.com/maps/place/Al+Thra'',+Amman/@31.9316088,35.8957457,14z/data=!3m1!4b1!4m5!3m4!1s0x151ca09e4dee477f:0x6b9bb2f38ff43098!8m2!3d31.931524!4d35.9161219?hl=en
', N'Al Thra''
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (138, N'الهلال
', 3, 20, N'https://www.google.com/maps/place/Al+Hilal,+Amman/@31.9301945,35.8789901,14z/data=!3m1!4b1!4m5!3m4!1s0x151ca0bed964824b:0x6deae69473b2a01!8m2!3d31.9294931!4d35.8997307?hl=en
', N'Al Hilal
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (139, N'الياسمين
', 3, 20, N'https://www.google.com/maps/place/Al+Yasmin,+Amman/@31.9179553,35.8732796,14z/data=!3m1!4b1!4m5!3m4!1s0x151ca0cae21bbe97:0xcc932d2acc34bb9c!8m2!3d31.9162302!4d35.8920658?hl=en
', N'Al Yasmin
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (140, N'الجندويل
', 3, 21, N'https://www.google.com/maps/place/Al+Jandawil,+Amman/@31.9624113,35.8319248,15z/data=!3m1!4b1!4m5!3m4!1s0x151ca17027399b3d:0x47daa3f5a2309002!8m2!3d31.9620609!4d35.8402813?hl=en
', N'Al Jandawil
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (141, N'الدمينة
', 3, 21, N'https://www.google.com/maps/place/Al+Dmenah,+Amman/@31.9253314,35.8233296,14z/data=!3m1!4b1!4m5!3m4!1s0x151ca6b1b2542dd1:0x9d1dc84b0abff218!8m2!3d31.9286824!4d35.8437335?hl=en
', N'Al Dmenah
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (142, N'الديار
', 3, 21, N'https://www.google.com/maps/place/Al+Diyar,+Amman/@31.939988,35.8462818,14z/data=!3m1!4b1!4m5!3m4!1s0x151ca1247f564ffd:0x2d86e3553ed08c70!8m2!3d31.943128!4d35.8624441?hl=en
', N'Al Diyar
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (143, N'الروابي
', 3, 21, N'https://www.google.com/maps/place/Al+Rawabi,+Amman/@31.9666097,35.8348903,14z/data=!3m1!4b1!4m5!3m4!1s0x151ca10af7785f4f:0x5a6ecfd916bdbcbb!8m2!3d31.965691!4d35.855686?hl=en
', N'Al Rawabi
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (144, N'السهل
', 3, 21, N'https://www.google.com/maps/place/Al+Sahel,+Amman/@31.9457032,35.8446111,15z/data=!3m1!4b1!4m5!3m4!1s0x151ca13ef7af81f1:0xdfe94fd0be6f5754!8m2!3d31.9543163!4d35.8560914?hl=en
', N'Al Sahel
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (145, N'الصناعة
', 3, 21, N'https://www.google.com/maps/place/Al+Sena''a,+Amman/@31.9372191,35.8187694,14z/data=!3m1!4b1!4m5!3m4!1s0x151ca1524d4410d7:0x9a51322bb21458b1!8m2!3d31.9399046!4d35.8424156?hl=en
', N'Al Sena''a
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (146, N'الصنوبر
', 3, 21, N'https://www.google.com/maps/place/Al+Snobr,+Amman/@31.934589,35.7938642,14z/data=!3m1!4b1!4m5!3m4!1s0x151ca40dd48576a3:0x9053c4fc872a514e!8m2!3d31.9310595!4d35.8094721?hl=en
', N'Al Snobr
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (147, N'الصويفية
', 3, 21, N'https://www.google.com/maps/place/Al+Swaifyeh,+Amman/@31.9529932,35.8554883,15z/data=!3m1!4b1!4m5!3m4!1s0x151ca11b10ef77b1:0xab1a6dc9f164d63f!8m2!3d31.9548429!4d35.8642572?hl=en
', N'Al Swaifyeh
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (148, N'الظهير
', 3, 21, N'https://www.google.com/maps/place/Al+Thahir,+Amman/@31.924404,35.8449595,15z/data=!3m1!4b1!4m5!3m4!1s0x151ca6cbc17a8cb5:0xe2cf0310e85460d!8m2!3d31.9163084!4d35.8538718?hl=en
', N'Al Thahir
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (149, N'الكرسي
', 3, 21, N'https://www.google.com/maps/place/Al+Kursi,+Amman/@31.9649579,35.807893,14z/data=!3m1!4b1!4m5!3m4!1s0x151ca3d8551f3609:0x56538b8f820bcb5c!8m2!3d31.9645466!4d35.8266649?hl=en
', N'Al Kursi
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (150, N'المدينة الطبية
', 3, 21, N'https://www.google.com/maps/place/Al+Madinah+Al+Tabyeh,+Amman/@31.9755044,35.813892,14z/data=!3m1!4b1!4m5!3m4!1s0x151ca181d744b997:0xafb68618e99903bd!8m2!3d31.9759128!4d35.8295581?hl=en
', N'Al Madinah Al Tabyeh
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (151, N'ام اذينة الغربي
', 3, 21, N'https://www.google.com/maps/place/Umm+Uthainah+Al+Gharbi,+Amman/@31.9672879,35.8592162,15z/data=!3m1!4b1!4m5!3m4!1s0x151ca1ab96ea1897:0x2fef127c9dfeb492!8m2!3d31.9679973!4d35.8679098
', N'Umm Uthainah Al Gharbi
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (152, N'البيادر
', 3, 21, N'https://www.google.com/maps/place/Al+Bayader,+Amman/@31.9418205,35.7335358,12z/data=!3m1!4b1!4m5!3m4!1s0x151ca47457c5585f:0x9ba111d416126471!8m2!3d31.9529717!4d35.8202638?hl=en
', N'Al Bayader
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (153, N'الرونق
', 3, 21, N'https://www.google.com/maps/place/Al+Rawnaq,+Amman/@31.9509054,35.8234608,14z/data=!3m1!4b1!4m5!3m4!1s0x151ca142ed893187:0x503abe45d585f731!8m2!3d31.9488126!4d35.8401359?hl=en
', N'Al Rawnaq
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (154, N'الرباحية
', 3, 22, N'https://www.google.com/maps/place/Al+Rabahieh,+Amman/@31.9704733,35.7934275,14z/data=!3m1!4b1!4m5!3m4!1s0x151ca3c4c94af4d1:0xa4b91b88586b2564!8m2!3d31.9732147!4d35.8147036?hl=en
', N'Al Rabahieh
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (155, N'السويسة
', 3, 22, N'https://www.google.com/maps/place/Al+Swaisah,+Amman/@31.9496303,35.7492106,14z/data=!3m1!4b1!4m5!3m4!1s0x151ca49973275f19:0x87572dcee2a0e970!8m2!3d31.9510931!4d35.7695819?hl=en
', N'Al Swaisah
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (156, N'الغروس
', 3, 22, N'https://www.google.com/maps/place/Al+Ghrous,+Amman/@31.9625607,35.7529102,14z/data=!3m1!4b1!4m5!3m4!1s0x151ca3645408420d:0x999d00209c3f2172!8m2!3d31.9610298!4d35.7753821?hl=en
', N'Al Ghrous
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (157, N'ام الاسود
', 3, 22, N'https://www.google.com/maps/place/Umm+A+Usoud,+Amman/@31.97745,35.7589743,13z/data=!3m1!4b1!4m5!3m4!1s0x151ca3a55fe03f63:0xf008c6fb68e78520!8m2!3d31.9767279!4d35.7872521?hl=en
', N'Umm A Usoud
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (158, N'بلال
', 3, 22, N'https://www.google.com/maps/place/Bilal,+Amman/@31.9633497,35.7585808,13z/data=!3m1!4b1!4m5!3m4!1s0x151ca3998e1266d9:0xe8a78b4a76af1f79!8m2!3d31.963153!4d35.7898607?hl=en
', N'Bilal
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (159, N'زبدة
', 3, 22, N'https://www.google.com/maps/place/Zebda,+Amman/@31.9496554,35.7183186,13z/data=!3m1!4b1!4m5!3m4!1s0x151ca4baebcc13cf:0xf74f455eed16a4dd!8m2!3d31.9457701!4d35.7545258?hl=en
', N'Zebda
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (160, N'الرضوان
', 3, 23, N'https://www.google.com/maps/place/Al+Radwan,+Amman/@31.956358,35.8606814,13z/data=!3m1!4b1!4m5!3m4!1s0x151ca060504a5bf3:0x478edbd304967718!8m2!3d31.9554886!4d35.8990183?hl=en
', N'Al Radwan
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (161, N'ام اذينة الشرقي
', 3, 23, N'https://www.google.com/maps/place/Umm+Uthainah+Al+Sharqi,+Amman/@31.9668847,35.8686095,15z/data=!3m1!4b1!4m5!3m4!1s0x151ca05163937b11:0x709332af3c7825fd!8m2!3d31.9668528!4d35.8746025?hl=en
', N'Umm Uthainah Al Sharqi
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (162, N'جبل عمان
', 3, 23, N'https://www.google.com/maps/place/Jabal+Amman,+Amman/@31.9528936,35.9019486,14z/data=!3m1!4b1!4m5!3m4!1s0x151b5f86956dc02d:0xa9edbdbd70c4d41c!8m2!3d31.9515589!4d35.9156256?hl=en
', N'Jabal Amman
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (163, N'عبدون الجنوبي
', 3, 23, N'https://www.google.com/maps/place/Abdun+Al+Janobi,+Amman/@31.937804,35.8529725,13z/data=!3m1!4b1!4m5!3m4!1s0x151ca0e9f7eeeb0b:0x17a950002a613f29!8m2!3d31.9410388!4d35.885213?hl=en
', N'Abdun Al Janobi
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (164, N'عبدون الشمالي
', 3, 23, N'https://www.google.com/maps/place/Abdun+Al+Shmali,+Amman/@31.9501374,35.8539693,13z/data=!3m1!4b1!4m5!3m4!1s0x151ca0f3c3f2d0dd:0x78f70515eb6b4cde!8m2!3d31.9477715!4d35.8920325?hl=en
', N'Abdun Al Shmali
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (165, N'البنيات الجنوبي
', 3, 24, N'https://www.google.com/maps/place/Al+Bunayyat+al+Janubiyah,+Amman/@31.8887981,35.8644675,14z/data=!3m1!4b1!4m5!3m4!1s0x151ca7055ae694ab:0xd1f8e2c76b9d9ed4!8m2!3d31.8870339!4d35.8796838?hl=en
', N'Al Bunayyat al Janubiyah
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (166, N'البنيات الشمالي
', 3, 24, N'https://www.google.com/maps/place/Al+Bunayyat+Al+Shamali,+Amman/@31.8905261,35.872931,14z/data=!3m1!4b1!4m5!3m4!1s0x151ca7139f59bc33:0x1a367ceedb283bf8!8m2!3d31.8910088!4d35.8868234?hl=en
', N'Al Bunayyat Al Shamali
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (167, N'الحرية
', 3, 24, N'https://www.google.com/maps/place/Al+Hurryeh,+Amman/@31.8972126,35.8540368,13z/data=!3m1!4b1!4m5!3m4!1s0x151ca73e1dfe978f:0xf3df0c50c09c68cc!8m2!3d31.9002187!4d35.8881105?hl=en
', N'Al Hurryeh
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (168, N'الحسنية
', 3, 24, N'https://www.google.com/maps/place/Al+Husnyeh,+Amman/@31.9081664,35.879876,14z/data=!3m1!4b1!4m5!3m4!1s0x151ca7496ae31885:0xa3569000dd7e5c1d!8m2!3d31.9085409!4d35.8999668?hl=en
', N'Al Husnyeh
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (169, N'الصحابة
', 3, 24, N'https://www.google.com/maps/place/Al+Sahabah,+Amman/@31.9143781,35.8543074,14z/data=!3m1!4b1!4m5!3m4!1s0x151ca72827ad0bb1:0x2b60c64d6083f08!8m2!3d31.907713!4d35.8731426?hl=en
', N'Al Sahabah
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (170, N'الكرامة
', 3, 24, N'https://www.google.com/maps/place/Al+Karamah,+Amman/@31.9243413,35.8500398,14z/data=!3m1!4b1!4m5!3m4!1s0x151ca12b49d97a95:0x234804c03a065ccf!8m2!3d31.9207981!4d35.8615843?hl=en
', N'Al Karamah
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (171, N'المقابلين
', 3, 24, N'https://www.google.com/maps/place/Al+Muqabalayn,+Amman/@31.9061604,35.9006495,14z/data=!3m1!4b1!4m5!3m4!1s0x151b58ab1f6f145b:0x2290efb6711624b1!8m2!3d31.9028057!4d35.9125538?hl=en
', N'Al Muqabalayn
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (172, N'أم قصير
', 3, 24, N'https://www.google.com/maps/place/Umm+Quseir,+Amman/@31.8909582,35.8847665,13z/data=!3m1!4b1!4m5!3m4!1s0x151b58a1420dce2f:0x51ce18f69227f268!8m2!3d31.8928643!4d35.9130235?hl=en
', N'Umm Quseir
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (173, N'الابرار
', 3, 25, N'https://www.google.com/maps/place/Al+Abrar,+Amman/@31.828992,35.8851171,14z/data=!3m1!4b1!4m5!3m4!1s0x151ca808cf568ecd:0xa5c78c2f1c347917!8m2!3d31.8341411!4d35.9020013?hl=en
', N'Al Abrar
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (174, N'الاحسان
', 3, 25, N'https://www.google.com/maps/place/Al+Ihsan,+Amman/@31.8602121,35.932776,14z/data=!3m1!4b1!4m5!3m4!1s0x151b58f9ac05c763:0xecdebd26a14e50ca!8m2!3d31.8643631!4d35.9506196?hl=en
', N'Al Ihsan
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (175, N'الامل
', 3, 25, N'https://www.google.com/maps/place/Al+Amal,+Amman/@31.8471216,35.86379,14z/data=!3m1!4b1!4m5!3m4!1s0x151ca7d08d6bfa47:0x75f81b41315d4cc1!8m2!3d31.8442094!4d35.883808?hl=en
', N'Al Amal
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (176, N'الاندلس
', 3, 25, N'https://www.google.com/maps/place/Al+Andalus,+Amman/@31.8795655,35.8934626,14z/data=!3m1!4b1!4m5!3m4!1s0x151ca77cb7712d97:0xc7dcb5a1cea8accc!8m2!3d31.8813385!4d35.9143824?hl=en
', N'Al Andalus
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (177, N'الايمان
', 3, 25, N'https://www.google.com/maps/place/Al+Iman,+Amman/@31.8726907,35.909206,13z/data=!3m1!4b1!4m5!3m4!1s0x151b58eeec206261:0x3edb0aa70276294!8m2!3d31.8738226!4d35.9445967?hl=en
', N'Al Iman
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (178, N'التقوى
', 3, 25, N'https://www.google.com/maps/place/Al+Taqwa,+Amman/@31.8683262,35.9383186,13z/data=!3m1!4b1!4m5!3m4!1s0x151b5916468b110f:0x12af9db573d0106a!8m2!3d31.8658253!4d35.9796273?hl=en
', N'Al Taqwa
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (179, N'الربيع
', 3, 25, N'https://www.google.com/maps/place/Al+Rabi,+Amman/@31.8178339,35.8848557,14z/data=!3m1!4b1!4m5!3m4!1s0x151ca8173ac4de1d:0x9d369aba121e817!8m2!3d31.807898!4d35.9034516?hl=en
', N'Al Rabi
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (180, N'الصفاء
', 3, 25, N'https://www.google.com/maps/place/Al+Safa,+Amman/@31.8243839,35.9006504,14z/data=!3m1!4b1!4m5!3m4!1s0x151b57fe8bb26a95:0x9577f0662ce20143!8m2!3d31.8215825!4d35.9126217?hl=en
', N'Al Safa
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (181, N'الطيبة وخريبة السوق
', 3, 25, N'https://www.google.com/maps/place/Al+Taibah+%26+Kherbet+Al+Souq,+Amman/@31.8747175,35.9047885,14z/data=!3m1!4b1!4m5!3m4!1s0x151b5884edb74315:0xb0ead1ae5794b57f!8m2!3d31.8704115!4d35.924588?hl=en
', N'Al Taibah & Kherbet Al Souq
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (182, N'الفرقان
', 3, 25, N'https://www.google.com/maps/place/Al+Furqan,+Amman/@31.9078884,35.8334679,12z/data=!4m5!3m4!1s0x151ca7757a0e2c43:0x1e82036a2fc7a8a!8m2!3d31.8805496!4d35.8950485?hl=en
', N'Al Furqan
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (183, N'المجد
', 3, 25, N'https://www.google.com/maps/place/Al+Majd,+Amman/@31.8653495,35.8541237,14z/data=!3m1!4b1!4m5!3m4!1s0x151ca64ad35684ff:0x3cd98fc706d338eb!8m2!3d31.8613865!4d35.8741552?hl=en
', N'Al Majd
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (184, N'الهداية
', 3, 25, N'https://www.google.com/maps/place/Al+Hidayeh,+Amman/@31.8350287,35.866632,14z/data=!3m1!4b1!4m5!3m4!1s0x151ca87739dafb3f:0xb07ee91a2945c326!8m2!3d31.8377688!4d35.8835194?hl=en
', N'Al Hidayeh
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (185, N'الوفاء
', 3, 25, N'https://www.google.com/maps/place/Al+Wafa,+Amman/@31.8435884,35.9334982,14z/data=!3m1!4b1!4m5!3m4!1s0x151b584eddfc5c69:0x945f4d16b2639b77!8m2!3d31.8418288!4d35.9486114?hl=en
', N'Al Wafa
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (186, N'اليادودة
', 3, 25, N'https://www.google.com/maps/place/Al+Yadudah,+Amman/@31.8505836,35.8972926,14z/data=!3m1!4b1!4m5!3m4!1s0x151ca78a991dfb81:0x398ce4a3fccd8e09!8m2!3d31.8512389!4d35.9167683?hl=en
', N'Al Yadudah
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (187, N'ام الكندم
', 3, 25, N'https://www.google.com/maps/place/Umm+al+Kundum,+Amman/@31.8698605,35.8625795,14z/data=!3m1!4b1!4m5!3m4!1s0x151ca7b02dc828cb:0x3d378af7db27507c!8m2!3d31.873726!4d35.883263?hl=en
', N'Umm al Kundum
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (188, N'جاوا الجنوبي
', 3, 25, N'https://www.google.com/maps/place/Jawa+Al+Janobi,+Amman/@31.8700224,35.8101777,12z/data=!4m5!3m4!1s0x151b5865e84fc03f:0x3eaee80f8fdb547f!8m2!3d31.8534626!4d35.9322584?hl=en
', N'Jawa Al Janobi
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (189, N'جاوا الشمالي
', 3, 25, N'https://www.google.com/maps/place/Jawa+Al+Shamali,+Amman/@31.8585782,35.9217588,14z/data=!3m1!4b1!4m5!3m4!1s0x151b585facd46477:0x2b2678a86cc7effa!8m2!3d31.8645037!4d35.9350129?hl=en
', N'Jawa Al Shamali
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (190, N'غمدان
', 3, 25, N'https://www.google.com/maps/place/Ghamadan,+Amman/@31.8585782,35.9217588,14z/data=!4m5!3m4!1s0x151ca7931dc3ef11:0x8948c70de1122335!8m2!3d31.8568662!4d35.8973801?hl=en
', N'Ghamadan
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (191, N'قباء
', 3, 25, N'https://www.google.com/maps/place/Qiba''a,+Amman/@31.8382755,35.9105674,14z/data=!3m1!4b1!4m5!3m4!1s0x151b5811bb1120cf:0xd2224c95bb575d65!8m2!3d31.8380952!4d35.9266087?hl=en
', N'Qiba''a
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (192, N'ارينبه الشرقية
', 3, 26, N'https://www.google.com/maps/place/Areinba+Al+Sharqiyah,+Amman/@31.6464031,35.9639653,13z/data=!3m1!4b1!4m5!3m4!1s0x151b54aba5fc28c1:0x3533403b1338af6a!8m2!3d31.6402724!4d35.9827422?hl=en
', N'Areinba Al Sharqiyah
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (193, N'الجيزه
', 3, 26, N'https://www.google.com/maps/place/Al+Jizah,+Amman/@31.7141508,35.6791345,10z/data=!3m1!4b1!4m5!3m4!1s0x151b55b52b7c7b3f:0xfc367e42c0cfee6b!8m2!3d31.700896!4d35.9510312?hl=en
', N'Al Jizah
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (194, N'الخضراء
', 3, 26, N'https://www.google.com/maps/place/Al+Khadra'',+Amman/@31.7787238,35.8392943,13z/data=!3m1!4b1!4m5!3m4!1s0x151ca99e7d3c0ba3:0x354314fe9cf819a2!8m2!3d31.787205!4d35.8795426?hl=en
', N'Al Khadra''
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (195, N'الزعفران
', 3, 26, N'https://www.google.com/maps/place/Al+Zafaran,+Amman/@31.6099448,35.835828,13z/data=!3m1!4b1!4m5!3m4!1s0x1503542b44fd87ef:0x4681afae43bc738e!8m2!3d31.614344!4d35.8725423?hl=en
', N'Al Zafaran
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (196, N'الزيتونه
', 3, 26, N'https://www.google.com/maps/place/Al+Zaytouneh,+Amman/@31.7486616,35.8627895,13z/data=!3m1!4b1!4m5!3m4!1s0x151caa1973bc86d1:0xa6189cb4b72a9a53!8m2!3d31.7578003!4d35.8982521?hl=en
', N'Al Zaytouneh
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (197, N'السيفية
', 3, 26, N'https://www.google.com/maps/place/Al+Sayfiyah,+Amman/@31.6657805,35.9867219,13z/data=!3m1!4b1!4m5!3m4!1s0x151b536d7e88457d:0x266bc92f0c024f69!8m2!3d31.6613675!4d36.024651?hl=en
', N'Al Sayfiyah
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (198, N'الصلاحيه
', 3, 26, N'https://www.google.com/maps/place/Al+Salahiyah,+Amman/@31.7164474,35.8899118,13z/data=!3m1!4b1!4m5!3m4!1s0x151b558542ec2673:0x3e640903cf84fd7d!8m2!3d31.7168258!4d35.9240895?hl=en
', N'Al Salahiyah
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (199, N'الطنيب
', 3, 26, N'https://www.google.com/maps/place/Al+Tuneib,+Amman/@31.7164474,35.8899118,13z/data=!4m5!3m4!1s0x151b57a3342b17f3:0xd0f23a5bfcfecc7b!8m2!3d31.8044945!4d35.9403631?hl=en
', N'Al Tuneib
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (200, N'الغبيه
', 3, 26, N'https://www.google.com/maps/place/Al+Ghubaiyah,+Amman/@31.7984258,35.8645097,14z/data=!3m1!4b1!4m5!3m4!1s0x151ca9ad7ae4eb61:0xb93dce43fb222456!8m2!3d31.7974395!4d35.8773626?hl=en
', N'Al Ghubaiyah
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (201, N'القسطل
', 3, 26, N'https://www.google.com/maps/place/Al+Qastal,+Amman/@31.7415471,35.8770565,12z/data=!3m1!4b1!4m5!3m4!1s0x151b567790c49285:0xaab7e04bc61b0fd2!8m2!3d31.7599879!4d35.940248?hl=en
', N'Al Qastal
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (202, N'القنيطره
', 3, 26, N'https://www.google.com/maps/place/Al+Quneitirah,+Amman/@31.6895499,35.9196505,11z/data=!3m1!4b1!4m5!3m4!1s0x151b523a34d9f447:0x725ede1d2576c2f0!8m2!3d31.670519!4d36.0590964?hl=en
', N'Al Quneitirah
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (203, N'اللبن
', 3, 26, N'https://www.google.com/maps/place/Al+Lubban,+Amman/@31.8200173,35.8946286,12z/data=!3m1!4b1!4m5!3m4!1s0x151b59d1636de08f:0xc07d366354e28f23!8m2!3d31.8239531!4d35.961683?hl=en
', N'Al Lubban
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (204, N'المشتى
', 3, 26, N'https://www.google.com/maps/place/Al+Mushatta,+Amman/@31.7432854,35.9334216,12z/data=!3m1!4b1!4m5!3m4!1s0x151b5135248fab6d:0xfdbc381fbe03e766!8m2!3d31.7436689!4d35.9973453?hl=en
', N'Al Mushatta
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (205, N'الهرى
', 3, 26, N'https://www.google.com/maps/place/Al+Heri,+Amman/@31.620407,35.8646291,13z/data=!3m1!4b1!4m5!3m4!1s0x15035593ee26a2df:0x3cc9182cb3f4a980!8m2!3d31.6247171!4d35.8911947?hl=en
', N'Al Heri
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (206, N'الهريج
', 3, 26, N'https://www.google.com/maps/place/Al+Hraij,+Amman/@31.6653779,35.8197701,14z/data=!3m1!4b1!4m5!3m4!1s0x150354853984a0a1:0xe9e5f1b10c546b7a!8m2!3d31.6595113!4d35.8353441?hl=en
', N'Al Hraij
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (207, N'ام العمد
', 3, 26, N'https://www.google.com/maps/place/Umm+Al+Amad,+Amman/@31.7855057,35.8709401,13z/data=!3m1!4b1!4m5!3m4!1s0x151ca9d1b431244b:0x4be201ab4d2f3bf4!8m2!3d31.7852267!4d35.9003007?hl=en
', N'Umm Al Amad
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (208, N'ام الوليد
', 3, 26, N'https://www.google.com/maps/place/Umm+Al+Walid,+Amman/@31.6442436,35.8664238,13z/data=!3m1!4b1!4m5!3m4!1s0x15035571ca7175a1:0x61d5857990b1c04b!8m2!3d31.6510252!4d35.8931384?hl=en
', N'Umm Al Walid
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (209, N'ام رمانه
', 3, 26, N'https://www.google.com/maps/place/Umm+Rummanah,+Amman/@31.6442436,35.8664238,13z/data=!4m5!3m4!1s0x151caa473218d71b:0xb2cc216861c31acc!8m2!3d31.7465565!4d35.8780409?hl=en
', N'Umm Rummanah
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (210, N'ام قصير
', 3, 26, N'https://www.google.com/maps/place/Umm+Quseir,+Amman/@31.7373395,35.8527132,13z/data=!4m5!3m4!1s0x151b58a1420dce2f:0x51ce18f69227f268!8m2!3d31.8928643!4d35.9130235?hl=en
', N'Umm Quseir
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (211, N'جلول
', 3, 26, N'https://www.google.com/maps/place/Jelul,+Amman/@31.7126341,35.7862851,12z/data=!3m1!4b1!4m5!3m4!1s0x151cab168efa7435:0x439d6135fb505b80!8m2!3d31.7168213!4d35.8514046?hl=en
', N'Jelul
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (212, N'حواره
', 3, 26, N'https://www.google.com/maps/place/Howarah,+Amman/@31.6891909,35.815666,13z/data=!3m1!4b1!4m5!3m4!1s0x150354ca883be42d:0xee500477d4264126!8m2!3d31.690841!4d35.8457387?hl=en
', N'Howarah
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (213, N'دليلة الحمايده
', 3, 26, N'https://www.google.com/maps/place/Dulaylat+Al+Hamaydeh,+Amman/@31.6380386,35.7605577,12z/data=!3m1!4b1!4m5!3m4!1s0x1503540ad7582f71:0xe852d888cc625f3!8m2!3d31.6414183!4d35.831781?hl=en
', N'Dulaylat Al Hamaydeh
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (214, N'زويزا
', 3, 26, N'https://www.google.com/maps/place/Zuwayza,+Amman/@31.6863504,35.841802,12z/data=!3m1!4b1!4m5!3m4!1s0x151caaa6ce526beb:0xa8457bed0bbb51d8!8m2!3d31.6970412!4d35.9141057?hl=en
', N'Zuwayza
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (215, N'زينب
', 3, 26, N'https://www.google.com/maps/place/Zeinab,+Amman/@31.6110439,35.8537551,12z/data=!3m1!4b1!4m5!3m4!1s0x1504aa72558af859:0xf1d22dc271680342!8m2!3d31.6145806!4d35.9149854?hl=en
', N'Zeinab
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (216, N'صوفا
', 3, 26, N'https://www.google.com/maps/place/Sufa,+Amman/@31.6676719,35.7802036,13z/data=!3m1!4b1!4m5!3m4!1s0x150353773a979cdd:0x8b2f90946f44f8e3!8m2!3d31.6623476!4d35.8182441?hl=en
', N'Sufa
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (217, N'منجا
', 3, 26, N'https://www.google.com/maps/place/Manja,+Amman/@31.6676719,35.7802036,13z/data=!4m5!3m4!1s0x151cabbe0da0abdf:0x17686dec35b29655!8m2!3d31.7449236!4d35.8544027?hl=en
', N'Manja
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (218, N'نتل
', 3, 26, N'https://www.google.com/maps/place/Netil,+Amman/@31.6538151,35.827978,13z/data=!3m1!4b1!4m5!3m4!1s0x150354fbe146ee7d:0x3e2e67673cb76f5!8m2!3d31.6496392!4d35.8633207?hl=en
', N'Netil
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (219, N'المطبة
', 3, 27, N'https://www.google.com/maps/place/Al+Matabba,+Amman/@31.7965075,35.9882317,11z/data=!3m1!4b1!4m5!3m4!1s0x151b450165134bdf:0x901759893498c562!8m2!3d31.796929!4d36.1855906?hl=en
', N'Al Matabba
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (220, N'الموقر
', 3, 27, N'https://www.google.com/maps/place/Al-Muwaqqar,+Amman/@31.810806,36.0574424,12z/data=!3m1!4b1!4m5!3m4!1s0x151b44d1697e0ddb:0x4c71b94df906dfb8!8m2!3d31.8120723!4d36.1062993?hl=en
', N'Al-Muwaqqar
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (221, N'النقيرة
', 3, 27, N'https://www.google.com/maps/place/Al+Nuqairah,+Amman/@31.8262014,35.9870743,12z/data=!3m1!4b1!4m5!3m4!1s0x151b5b1ccc77cb63:0xe29ee2ffba7153aa!8m2!3d31.8508401!4d36.0539984?hl=en
', N'Al Nuqairah
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (222, N'ذهيبه الشرقية
', 3, 27, N'https://www.google.com/maps/place/Dhuheibah+Al+Sharqiyah,+Amman/@31.7842056,36.0021897,13z/data=!3m1!4b1!4m5!3m4!1s0x151b5084851cf411:0xff486cde133b3fb7!8m2!3d31.8032307!4d36.0247621?hl=en
', N'Dhuheibah Al Sharqiyah
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (223, N'ذهيبه الغربية
', 3, 27, N'https://www.google.com/maps/place/Dhuheibah+Al+Gharbiyah,+Amman/@31.7903637,35.963135,13z/data=!3m1!4b1!4m5!3m4!1s0x151b574dc90aa321:0xb748d84d576aa0d4!8m2!3d31.7991261!4d36.0039608?hl=en
', N'Dhuheibah Al Gharbiyah
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (224, N'رجم الشامي
', 3, 27, N'https://www.google.com/maps/place/Rujm+ash+Shami,+Amman/@31.8269643,35.9748127,13z/data=!3m1!4b1!4m5!3m4!1s0x151b5a25a2a9bc03:0xd01eb2221ca05754!8m2!3d31.8353998!4d36.0014693?hl=en
', N'Rujm ash Shami
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (225, N'سالم
', 3, 27, N'https://www.google.com/maps/place/Salem,+Amman/@31.8475457,35.9422084,13z/data=!3m1!4b1!4m5!3m4!1s0x151b59a08d40f0a5:0x75483ebcc378d31b!8m2!3d31.8517446!4d35.9724602?hl=en
', N'Salem
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (226, N'أبو صوانة
', 3, 28, N'https://www.google.com/maps/place/Abu+Sowaneh,+Amman/@31.8798326,35.9642573,14z/data=!3m1!4b1!4m5!3m4!1s0x151b5949c8535d11:0x66bd3ad955a6433b!8m2!3d31.8805692!4d35.9830448?hl=en
', N'Abu Sowaneh
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (227, N'ابو علندا
', 3, 28, N'https://www.google.com/maps/place/Abu+Alanda,+Amman/@31.9038588,35.9332466,13z/data=!3m1!4b1!4m5!3m4!1s0x151b5edb1f50337d:0x2e02bd84955482f8!8m2!3d31.9028195!4d35.962599?hl=en
', N'Abu Alanda
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (228, N'الجويدة
', 3, 28, N'https://www.google.com/maps/place/Al+Jwaideh,+Amman/@31.8894647,35.9279888,14z/data=!3m1!4b1!4m5!3m4!1s0x151b58cf9c807253:0x667836d99dbd57d9!8m2!3d31.8872767!4d35.9415034?hl=en
', N'Al Jwaideh
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (229, N'الرقيم
', 3, 28, N'https://www.google.com/maps/place/Al+Raqim,+Amman/@31.8894647,35.9279888,14z/data=!4m5!3m4!1s0x151b5eb6bad06aaf:0x1a3a9498851793aa!8m2!3d31.8923841!4d35.9862926?hl=en
', N'Al Raqim
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (230, N'العروبة
', 3, 28, N'https://www.google.com/maps/place/Al+Oroubah,+Amman/@31.9210647,35.9591363,14z/data=!3m1!4b1!4m5!3m4!1s0x151b5ef3b70021c1:0x86fa7fc9356decaf!8m2!3d31.9167751!4d35.9708292?hl=en
', N'Al Oroubah
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (231, N'القويسمة
', 3, 28, N'https://www.google.com/maps/place/Al+Quwaysimah,+Amman/@31.9105154,35.9318254,14z/data=!3m1!4b1!4m5!3m4!1s0x151b5f24e9b7e715:0x3efe7091b9a25af7!8m2!3d31.9100651!4d35.949099?hl=en
', N'Al Quwaysimah
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (232, N'المعادي
', 3, 28, N'https://www.google.com/maps/place/Al+Ma''adi,+Amman/@31.9139223,35.9430134,14z/data=!3m1!4b1!4m5!3m4!1s0x151b5ee07170867b:0x82c60b7f8d0b14e0!8m2!3d31.9143351!4d35.9603189?hl=en
', N'Al Ma''adi
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (233, N'المغبة الشرقي
', 3, 28, N'https://www.google.com/maps/place/Al+Maghaba+Al+Sharqi,+Amman/@31.8848745,35.95483,14z/data=!3m1!4b1!4m5!3m4!1s0x151b5931800fbdfd:0xc9ff915951afa92a!8m2!3d31.8854645!4d35.9728489?hl=en
', N'Al Maghaba Al Sharqi
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (234, N'المغبة الغربي
', 3, 28, N'https://www.google.com/maps/place/Al+Mghaba+Al+Gahrbi,+Amman/@31.888272,35.947507,14z/data=!3m1!4b1!4m5!3m4!1s0x151b592c123bc867:0x3f76f1793fd6d815!8m2!3d31.8928931!4d35.9657804?hl=en
', N'Al Mghaba Al Gahrbi
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (235, N'النهارية
', 3, 28, N'https://www.google.com/maps/place/Al+Naharyah,+Amman/@31.9288424,35.9310326,14z/data=!3m1!4b1!4m5!3m4!1s0x151b5f0565fc0c19:0x3d4d2be68aaf137b!8m2!3d31.9267814!4d35.9471172?hl=en
', N'Al Naharyah
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (236, N'أم الحيران
', 3, 28, N'https://www.google.com/maps/place/Umm+Al+Hiran,+Amman/@31.9113977,35.9289737,14z/data=!3m1!4b1!4m5!3m4!1s0x151b5f3b17d278f5:0x9c4638d844528261!8m2!3d31.9072332!4d35.9392537?hl=en
', N'Umm Al Hiran
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (237, N'ام نوارة
', 3, 28, N'https://www.google.com/maps/place/Umm+Nowarah,+Amman/@31.9300854,35.9456527,14z/data=!3m1!4b1!4m5!3m4!1s0x151b5eff20d79727:0x2dd707bdb2fb9222!8m2!3d31.9306595!4d35.9608376?hl=en
', N'Umm Nowarah
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (238, N'حطين
', 3, 28, N'https://www.google.com/maps/place/Hettin,+Amman/@31.8892199,35.9119454,14z/data=!3m1!4b1!4m5!3m4!1s0x151b58bb8610e4e9:0x15d1f41cb494b7a2!8m2!3d31.8922036!4d35.9279788?hl=en
', N'Hettin
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (239, N'معامل الطوب
', 3, 29, N'https://www.google.com/maps/place/Ma''amel+Al+Toob,+Zarqa/@32.1128865,36.0751402,14z/data=!3m1!4b1!4m5!3m4!1s0x151b7076856a1cef:0x85f9ca95e5d522d!8m2!3d32.1130379!4d36.0955396?hl=en
', N'Ma''amel Al Toob
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (240, N'الهاشمية الجنوبية
', 3, 29, N'https://www.google.com/maps/place/Al+Hashimeiyah+Al+Janobiyah,+Zarqa/@32.105317,36.0979568,15z/data=!3m1!4b1!4m5!3m4!1s0x151b706b9e6d99a5:0x80006cc918976519!8m2!3d32.1061923!4d36.1067525?hl=en
', N'Al Hashimeiyah Al Janobiyah
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (241, N'الحرفيين
', 3, 29, N'https://www.google.com/maps/place/Al+Herafien,+Zarqa/@32.1016666,36.1022041,15z/data=!3m1!4b1!4m5!3m4!1s0x151b706aaa27b45f:0x21bb29d009753e27!8m2!3d32.1029327!4d36.1105629?hl=en
', N'Al Herafien
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (242, N'البتراوي
', 3, 29, N'https://www.google.com/maps/place/Al+Batrawi,+Zarqa/@32.1006513,36.0553205,13z/data=!3m1!4b1!4m5!3m4!1s0x151b7008337ea983:0xc35f423f997645c!8m2!3d32.0936546!4d36.0874935?hl=en
', N'Al Batrawi
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (243, N'الزرقاء الجديدة
', 3, 29, N'https://www.google.com/maps/place/New+Zarqa,+Zarqa/@32.0912376,36.0599192,13z/data=!3m1!4b1!4m5!3m4!1s0x151b7003e2d66907:0xaa93aa6eca2e58b8!8m2!3d32.091878!4d36.1034456?hl=en
', N'New Zarqa
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (244, N'الهاشمي
', 3, 29, N'https://www.google.com/maps/place/Al+Hashimi,+Zarqa/@32.0942761,36.0569589,14z/data=!3m1!4b1!4m5!3m4!1s0x151b7ab3381847cf:0xd120d3c07c53acf7!8m2!3d32.0972654!4d36.0672737?hl=en
', N'Al Hashimi
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (245, N'البستان
', 3, 29, N'https://www.google.com/maps/place/Al+Bustan,+Zarqa/@32.1030614,36.0350991,14z/data=!3m1!4b1!4m5!3m4!1s0x151b7ad374cfb5e9:0xfe717f8bc92f372a!8m2!3d32.1065068!4d36.0545344?hl=en
', N'Al Bustan
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (246, N'شومر
', 3, 29, N'https://www.google.com/maps/place/Shomar,+Zarqa/@32.0915971,36.01729,13z/data=!3m1!4b1!4m5!3m4!1s0x151b652f5ca03817:0xb370738223726ba0!8m2!3d32.0828199!4d36.0561697?hl=en
', N'Shomar
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (247, N'الجنينه
', 3, 29, N'https://www.google.com/maps/place/Al+Jenenah,+Zarqa/@32.0773871,36.0527328,15z/data=!3m1!4b1!4m5!3m4!1s0x151b65160fd7a1f3:0xce2d5af23a42780f!8m2!3d32.0761195!4d36.0621499?hl=en
', N'Al Jenenah
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (248, N'الزواهره
', 3, 29, N'https://www.google.com/maps/place/Al+Zawahrah,+Zarqa/@32.0685712,36.0514815,14z/data=!3m1!4b1!4m5!3m4!1s0x151b656ce0addd1f:0xbb385f7a18c3a7d3!8m2!3d32.0667188!4d36.0668621?hl=en
', N'Al Zawahrah
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (249, N'القمر
', 3, 29, N'https://www.google.com/maps/place/Al+Qamar,+Zarqa/@32.0604503,36.0524515,15z/data=!3m1!4b1!4m5!3m4!1s0x151b65099a36df45:0x653e5c2bd5991578!8m2!3d32.0611119!4d36.0618622?hl=en
', N'Al Qamar
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (250, N'قرطبه
', 3, 29, N'https://www.google.com/maps/place/Qurtobah,+Zarqa/@32.0690213,36.0492025,15z/data=!3m1!4b1!4m5!3m4!1s0x151b650fe717e9c1:0x4559905f0f9c3cfb!8m2!3d32.068216!4d36.0569906?hl=en
', N'Qurtobah
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (251, N'الجبر
', 3, 29, N'https://www.google.com/maps/place/Al+Jabir,+Zarqa/@32.0723235,36.0349758,14z/data=!3m1!4b1!4m5!3m4!1s0x151b651b405397e9:0xedd0b8d753e30499!8m2!3d32.068549!4d36.052592?hl=en
', N'Al Jabir
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (252, N'الاميره هيا
', 3, 29, N'https://www.google.com/maps/place/Al+Amirah+Haia,+Zarqa/@32.0743086,36.0303279,14z/data=!3m1!4b1!4m5!3m4!1s0x151b651d10b1e829:0x6c5b2d3fe3307a32!8m2!3d32.0678214!4d36.0433644?hl=en
', N'Al Amirah Haia
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (253, N'معصوم
', 3, 29, N'https://www.google.com/maps/place/Ma''asom,+Zarqa/@32.0802589,36.0678666,15z/data=!3m1!4b1!4m5!3m4!1s0x151b6544e3875527:0xb531e0c8617f0eaf!8m2!3d32.0809365!4d36.0771736?hl=en
', N'Ma''asom
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (254, N'البساتين
', 3, 29, N'https://www.google.com/maps/place/Al+Basatin,+Zarqa/@32.0753188,36.0672985,16z/data=!3m1!4b1!4m5!3m4!1s0x151b6541e7f54feb:0x168b2898b1c6ed12!8m2!3d32.0776095!4d36.0718369?hl=en
', N'Al Basatin
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (255, N'الامير شاكر
', 3, 29, N'https://www.google.com/maps/place/Al+Amir+Shaker,+Zarqa/@32.0734009,36.0686225,15z/data=!3m1!4b1!4m5!3m4!1s0x151b6542a373853b:0x9a57c2e6fcf3060c!8m2!3d32.0729519!4d36.0764247?hl=en
', N'Al Amir Shaker
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (256, N'الاسكان
', 3, 29, N'https://www.google.com/maps/place/Al+Iskan,+Zarqa/@32.0869018,36.0950013,15z/data=!3m1!4b1!4m5!3m4!1s0x151b701c4e512b43:0x28a8e90c8853e03d!8m2!3d32.0872108!4d36.1050691?hl=en
', N'Al Iskan
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (257, N'ابن سينا
', 3, 29, N'https://www.google.com/maps/place/Iben+Sina,+Zarqa/@32.0825227,36.0947722,15z/data=!3m1!4b1!4m5!3m4!1s0x151b6fe25b5e36cf:0x917ac680cb0701bc!8m2!3d32.0817645!4d36.1012947?hl=en
', N'Iben Sina
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (258, N'الشيوخ
', 3, 29, N'https://www.google.com/maps/place/Al+Shiokh,+Zarqa/@32.0782244,36.0927064,15z/data=!3m1!4b1!4m5!3m4!1s0x151b6fe4c23bf213:0x1cdd77a569bec735!8m2!3d32.0795584!4d36.0997693?hl=en
', N'Al Shiokh
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (259, N'برخ
', 3, 29, N'https://www.google.com/maps/place/Barkh,+Zarqa/@32.0840607,36.0870238,15z/data=!3m1!4b1!4m5!3m4!1s0x151b6ffc33613fab:0xe3628adb91c4e74a!8m2!3d32.0831612!4d36.0969679?hl=en
', N'Barkh
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (260, N'الامير محمد
', 3, 29, N'https://www.google.com/maps/place/Al+Amir+Mohammad,+Zarqa/@32.0791387,36.0850179,15z/data=!3m1!4b1!4m5!3m4!1s0x151b6ff990d24003:0xfc5e00d15a9adbcb!8m2!3d32.0795781!4d36.0938939?hl=en
', N'Al Amir Mohammad
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (261, N'رمزي
', 3, 29, N'https://www.google.com/maps/place/Ramzi,+Zarqa/@32.0766838,36.0773482,15z/data=!3m1!4b1!4m5!3m4!1s0x151b65591a5577bd:0x3b7097095aa79c98!8m2!3d32.0730628!4d36.0858591?hl=en
', N'Ramzi
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (262, N'الغويرية
', 3, 29, N'https://www.google.com/maps/place/Al+Ghwariyah,+Zarqa/@32.0728379,36.0790007,14z/data=!3m1!4b1!4m5!3m4!1s0x151b6ff113525933:0xf2d84f597620d97!8m2!3d32.0746712!4d36.0949532?hl=en
', N'Al Ghwariyah
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (263, N'النصر
', 3, 29, N'https://www.google.com/maps/place/Al+Nasir,+Zarqa/@32.0666544,36.0860699,15z/data=!3m1!4b1!4m5!3m4!1s0x151b6f8ca817de9b:0x15987756147c20f1!8m2!3d32.0679109!4d36.096642?hl=en
', N'Al Nasir
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (264, N'الحسين
', 3, 29, N'https://www.google.com/maps/place/Al+Hussein,+Zarqa/@32.065378,36.0712337,15z/data=!3m1!4b1!4m5!3m4!1s0x151b6565bd1db179:0x70c34ef3d80453b6!8m2!3d32.0657207!4d36.0798281?hl=en
', N'Al Hussein
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (265, N'النزهه
', 3, 29, N'https://www.google.com/maps/place/Al+Nozha,+Zarqa/@32.0622793,36.0704782,15z/data=!3m1!4b1!4m5!3m4!1s0x151b657ab2600aa5:0x221daf34da342a70!8m2!3d32.0629384!4d36.0784577?hl=en
', N'Al Nozha
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (266, N'الحديقة
', 3, 29, N'https://www.google.com/maps/place/Al+Hadiqa,+Zarqa/@32.0599441,36.0801788,16z/data=!3m1!4b1!4m5!3m4!1s0x151b657ca52e5b15:0x99b7be28206d2727!8m2!3d32.0600124!4d36.0855295?hl=en
', N'Al Hadiqa
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (267, N'الوسط التجاري
', 3, 29, N'https://www.google.com/maps/place/Al+Wasat+Al+Tijari,+Zarqa/@32.0634997,36.0811929,15z/data=!3m1!4b1!4m5!3m4!1s0x151b6f89fa16f3cf:0xb724346480ceb395!8m2!3d32.0634636!4d36.0891199?hl=en
', N'Al Wasat Al Tijari
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (268, N'الضباط
', 3, 29, N'https://www.google.com/maps/place/Al+Dobat,+Zarqa/@32.0581733,36.0806693,15z/data=!3m1!4b1!4m5!3m4!1s0x151b6f87f60aae85:0x1bd279d502a12910!8m2!3d32.0575318!4d36.0872708?hl=en
', N'Al Dobat
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (269, N'جناعه
', 3, 29, N'https://www.google.com/maps/place/Jannaa''ah,+Zarqa/@32.0542092,36.081381,15z/data=!3m1!4b1!4m5!3m4!1s0x151b6f874ddbac09:0x1445e2711190db2!8m2!3d32.0533159!4d36.0891157?hl=en
', N'Jannaa''ah
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (270, N'اسكان التطوير الحضري
', 3, 29, N'https://www.google.com/maps/place/Eskan+Al+Tatwir+Al+Hadari,+Zarqa/@32.0542092,36.081381,15z/data=!4m5!3m4!1s0x151b6f70e3d561ef:0xcaa5615493306a02!8m2!3d32.03802!4d36.1005228?hl=en
', N'Eskan Al Tatwir Al Hadari
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (271, N'طارق بن زياد
', 3, 29, N'https://www.google.com/maps/place/Tareq+Ben+Ziyad,+Zarqa/@32.0398011,36.0842541,15z/data=!3m1!4b1!4m5!3m4!1s0x151b6f7cee95f315:0x391b5d3037c87ea4!8m2!3d32.0404234!4d36.0948274?hl=en
', N'Tareq Ben Ziyad
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (272, N'الثوره العربية الكبرى
', 3, 29, N'https://www.google.com/maps/place/Al+Thawrah+Al+Arabia+Al+Kubrah,+Zarqa/@32.0427261,36.0686942,14z/data=!3m1!4b1!4m5!3m4!1s0x151b6586568b6a21:0x7ab049c1fa1baa69!8m2!3d32.0371451!4d36.0887943?hl=en
', N'Al Thawrah Al Arabia Al Kubrah
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (273, N'الجندي
', 3, 29, N'https://www.google.com/maps/place/Al+Jundi,+Zarqa/@32.0264557,36.0630475,14z/data=!3m1!4b1!4m5!3m4!1s0x151b65f199c31b73:0x5c6eb0a5cc68c091!8m2!3d32.0264392!4d36.0798278?hl=en
', N'Al Jundi
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (274, N'المصانع
', 3, 29, N'https://www.google.com/maps/place/Al+Masana'',+Zarqa/@32.0224938,36.0583,13z/data=!3m1!4b1!4m5!3m4!1s0x151b6f5938ec6519:0xac5d2634dacda1ec!8m2!3d32.0270596!4d36.0919652?hl=en
', N'Al Masana''
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (275, N'نصار
', 3, 29, N'https://www.google.com/maps/place/Nassar,+Zarqa/@32.0694612,36.0263243,15z/data=!3m1!4b1!4m5!3m4!1s0x151b64f82acf6f73:0xdfd3ff5a852558a9!8m2!3d32.070448!4d36.037268?hl=en
', N'Nassar
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (276, N'الاحمد
', 3, 29, N'https://www.google.com/maps/place/Al+Ahmad,+Zarqa/@32.0602308,36.0199019,14z/data=!3m1!4b1!4m5!3m4!1s0x151b6450de660de7:0x73d81aa93dd55153!8m2!3d32.0635746!4d36.0400968?hl=en
', N'Al Ahmad
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (277, N'مكة المكرمه
', 3, 29, N'https://www.google.com/maps/place/Makka+Al+Mokarameh,+Zarqa/@32.0479475,36.0172291,14z/data=!3m1!4b1!4m5!3m4!1s0x151b6447fce9d8d1:0x3bfdb06c841aedee!8m2!3d32.0474684!4d36.041007?hl=en
', N'Makka Al Mokarameh
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (278, N'المدينه المنوره
', 3, 29, N'https://www.google.com/maps/place/Al+Madinah+Al+Monawarah,+Zarqa/@32.056569,36.0318187,14z/data=!3m1!4b1!4m5!3m4!1s0x151b65ac6f3882f9:0x5558234fb98aeb88!8m2!3d32.0574556!4d36.0542642?hl=en
', N'Al Madinah Al Monawarah
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (279, N'ام بياضه
', 3, 29, N'https://www.google.com/maps/place/Umm+Baiadah,+Zarqa/@32.0534686,36.0538973,15z/data=!3m1!4b1!4m5!3m4!1s0x151b65a1511e9feb:0x2ff2229ea11271a!8m2!3d32.052385!4d36.0629595?hl=en
', N'Umm Baiadah
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (280, N'عوجان
', 3, 29, N'https://www.google.com/maps/place/Awajan/@32.0440594,36.0392876,14z/data=!4m5!3m4!1s0x151b65e8658e14ef:0xa96c1dd1098a4320!8m2!3d32.0271756!4d36.0695558?hl=en
', N'Awajan
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (281, N'الامير حمزه
', 3, 29, N'https://www.google.com/maps/place/Al+Amir+Hamza,+Zarqa/@32.055556,36.0642369,15z/data=!3m1!4b1!4m5!3m4!1s0x151b6575e1f9fa2b:0x8f20941b38f96195!8m2!3d32.05531!4d36.0713603?hl=en
', N'Al Amir Hamza
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (282, N'الدويك
', 3, 29, N'https://www.google.com/maps/place/Al+Duweik,+Zarqa/@32.0469883,36.0368326,14z/data=!3m1!4b1!4m5!3m4!1s0x151b65b0dc29fc6b:0x9509b6be5c2e911f!8m2!3d32.04687!4d36.056379?hl=en
', N'Al Duweik
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (283, N'الفلاح
', 3, 29, N'https://www.google.com/maps/place/Al+Falah,+Zarqa/@32.039723,36.0491909,14z/data=!3m1!4b1!4m5!3m4!1s0x151b65be3162a629:0xbc2eb5e6ebcf0805!8m2!3d32.034478!4d36.0620143?hl=en
', N'Al Falah
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (284, N'الجبل الابيض
', 3, 29, N'https://www.google.com/maps/place/Al+Jabal+Al+Abyad,+Zarqa/@32.0545298,36.0697084,15z/data=!3m1!4b1!4m5!3m4!1s0x151b6577882a41d7:0x46b19546c018e81e!8m2!3d32.0520112!4d36.0793312?hl=en
', N'Al Jabal Al Abyad
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (285, N'الاميره رحمه
', 3, 29, N'https://www.google.com/maps/place/Al+Amirah+Rahmah,+Zarqa/@32.0454925,36.0699618,15z/data=!3m1!4b1!4m5!3m4!1s0x151b6584d44ae96d:0xbc3ce7b2c2a8e415!8m2!3d32.044235!4d36.0784842?hl=en
', N'Al Amirah Rahmah
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (286, N'الامير حسن
', 3, 29, N'https://www.google.com/maps/place/Al+Amir+Hassan,+Zarqa/@32.038716,36.0689289,15z/data=!3m1!4b1!4m5!3m4!1s0x151b6591f0f4211f:0xc4dd19269aba5a90!8m2!3d32.0385287!4d36.0771941?hl=en
', N'Al Amir Hassan
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (287, N'الملك طلال
', 3, 29, N'https://www.google.com/maps/place/Al+Malik+Talal,+Zarqa/@32.0341005,36.065548,15z/data=!3m1!4b1!4m5!3m4!1s0x151b65ecb6732767:0x2d87361b4117b4f9!8m2!3d32.0344535!4d36.0737802?hl=en
', N'Al Malik Talal
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (288, N'حي الحرمين الغربي
', 3, 30, N'https://www.google.com/maps/place/Al+Herafyen+Al+Gharbi,+Irbid/@32.5772188,35.8332164,16z/data=!3m1!4b1!4m5!3m4!1s0x151c740fc7eef749:0xef530baa62251387!8m2!3d32.5752434!4d35.8372207?hl=en
', N'Al Herafyen Al Gharbi
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (289, N'حي المرج
', 3, 30, N'https://www.google.com/maps/place/Al+Marrj,+Irbid/@32.5712113,35.8290772,15z/data=!3m1!4b1!4m5!3m4!1s0x151c7404bc631863:0xbaf1285d04f5cddf!8m2!3d32.5729364!4d35.837315?hl=en
', N'Al Marrj
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (290, N'حي السعادة
', 3, 30, N'https://www.google.com/maps/place/Al+Sa''adah,+Irbid/@32.5714698,35.8239,15z/data=!3m1!4b1!4m5!3m4!1s0x151c7401456f68d9:0x905808a3c63c8c32!8m2!3d32.5696999!4d35.831332?hl=en
', N'Al Sa''adah
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (291, N'حي الاشرفية
', 3, 30, N'https://www.google.com/maps/place/Al+Ashrafiyah,+Irbid/@32.5729377,35.8168109,15z/data=!3m1!4b1!4m5!3m4!1s0x151c73ff1ea583f7:0xf948eef079f6149b!8m2!3d32.5753127!4d35.8242671?hl=en
', N'Al Ashrafiyah
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (292, N'حي البساتين
', 3, 30, N'https://www.google.com/maps/place/Al+Basatin,+Irbid/@32.5678058,35.8031873,14z/data=!3m1!4b1!4m5!3m4!1s0x151c7156eedb9ad7:0x1917521a63aa257!8m2!3d32.5677709!4d35.8207128?hl=en
', N'Al Basatin
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (293, N'حي الصوانية
', 3, 30, N'https://www.google.com/maps/place/Al+Swaniyah,+Irbid/@32.5585818,35.8137355,15z/data=!3m1!4b1!4m5!3m4!1s0x151c714e78bb7391:0x203d415ff454680d!8m2!3d32.5599381!4d35.8237318?hl=en
', N'Al Swaniyah
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (294, N'حي المطلع
', 3, 30, N'https://www.google.com/maps/place/Al+Matla'',+Irbid/@32.5635275,35.8206533,14z/data=!3m1!4b1!4m5!3m4!1s0x151c76a86544c7af:0x535ab38ed5dd5431!8m2!3d32.5629331!4d35.8385869?hl=en
', N'Al Matla''
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (295, N'حي الصحة
', 3, 30, N'https://www.google.com/maps/place/Al+Seha,+Irbid/@32.5599573,35.8192144,14z/data=!3m1!4b1!4m5!3m4!1s0x151c76af230d150f:0x9a32b88ecd1291e9!8m2!3d32.5593198!4d35.8376952?hl=en
', N'Al Seha
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (296, N'حي الحرفين الشرقي
', 3, 30, N'https://www.google.com/maps/place/Al+Herafyen+Al+Shareqi,+Irbid/@32.5772512,35.8380426,16z/data=!3m1!4b1!4m5!3m4!1s0x151c741112246d99:0x90936b684c5facaf!8m2!3d32.5772506!4d35.8422889?hl=en
', N'Al Herafyen Al Shareqi
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (297, N'حي اليرموك
', 3, 30, N'https://www.google.com/maps/place/Al+Yarmouk,+Irbid/@32.5787268,35.8376112,15z/data=!3m1!4b1!4m5!3m4!1s0x151c741447f99985:0xbae35677ba487970!8m2!3d32.5788525!4d35.8474114?hl=en
', N'Al Yarmouk
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (298, N'حي حنينا
', 3, 30, N'https://www.google.com/maps/place/Hanina,+Irbid/@32.5779281,35.8423741,14z/data=!3m1!4b1!4m5!3m4!1s0x151c74380836baa9:0xd7f70d9decbc6fc2!8m2!3d32.5777507!4d35.8577958?hl=en
', N'Hanina
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (299, N'حي النصر
', 3, 30, N'https://www.google.com/maps/place/Al+Naser,+Irbid/@32.5708964,35.8379225,15z/data=!3m1!4b1!4m5!3m4!1s0x151c7419b14eab41:0x7987599516c649b5!8m2!3d32.5704363!4d35.8472608?hl=en
', N'Al Naser
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (300, N'حي الكرامة
', 3, 30, N'https://www.google.com/maps/place/Al+Karama,+Irbid/@32.5735081,35.8411963,14z/data=!3m1!4b1!4m5!3m4!1s0x151c7439880e0ebb:0x3db2fccbe04aeaa2!8m2!3d32.5711721!4d35.8588232?hl=en
', N'Al Karama
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (301, N'حي البياضة
', 3, 30, N'https://www.google.com/maps/place/Al+Bayadah,+Irbid/@32.5776469,35.8647544,16z/data=!3m1!4b1!4m5!3m4!1s0x151c744b01391b53:0xc6525d2a5735b4ce!8m2!3d32.5768649!4d35.8689608?hl=en
', N'Al Bayadah
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (302, N'حي البقعة
', 3, 30, N'https://www.google.com/maps/place/Al+Baqa''ah,+Irbid/@32.5737522,35.8687319,15z/data=!3m1!4b1!4m5!3m4!1s0x151c75b6715e0ee7:0xcb57e4499e500c4c!8m2!3d32.5719195!4d35.8740834?hl=en
', N'Al Baqa''ah
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (303, N'حي الصناعة
', 3, 30, N'https://www.google.com/maps/place/Al+Senaa''ah,+Irbid/@32.5738374,35.8424671,13z/data=!4m5!3m4!1s0x151c7435b0c82cf9:0x2bd3a0fdc7cf8464!8m2!3d32.5724527!4d35.868552?hl=en
', N'Al Senaa''ah
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (304, N'حي الايمان
', 3, 30, N'https://www.google.com/maps/place/Al+Iman,+Irbid/@32.5738374,35.8424671,13z/data=!4m5!3m4!1s0x151c7430fe958ab5:0x241b5e773b5d81e3!8m2!3d32.5699086!4d35.8651078?hl=en
', N'Al Iman
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (305, N'حي الروضة
', 3, 30, N'https://www.google.com/maps/place/Al+Rawdah,+Irbid/@32.5644493,35.8692689,15z/data=!3m1!4b1!4m5!3m4!1s0x151c75ce63cf3601:0xbd52e3209cfc5959!8m2!3d32.5649815!4d35.8781709?hl=en
', N'Al Rawdah
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (306, N'حي السهل الاخضر
', 3, 30, N'https://www.google.com/maps/place/Al+Sahel+Al+Akhdar,+Irbid/@32.5629149,35.859199,15z/data=!3m1!4b1!4m5!3m4!1s0x151c742c4ceccf73:0xd419d847ec7051cd!8m2!3d32.5620091!4d35.8673745?hl=en
', N'Al Sahel Al Akhdar
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (307, N'حي السلام
', 3, 30, N'https://www.google.com/maps/place/Al+Salam,+Irbid/@32.5625844,35.849899,15z/data=!3m1!4b1!4m5!3m4!1s0x151c74266626d01f:0xa869fb6b7f341cec!8m2!3d32.563244!4d35.8586294?hl=en
', N'Al Salam
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (308, N'حي الميدان
', 3, 30, N'https://www.google.com/maps/place/Al+Maydan,+Irbid/@32.5597639,35.852775,16z/data=!3m1!4b1!4m5!3m4!1s0x151c7427657eb1d3:0x587fcb0c5a9bdcc0!8m2!3d32.5603827!4d35.8569116?hl=en
', N'Al Maydan
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (309, N'حي العودة
', 3, 30, N'https://www.google.com/maps/place/Al+A''awdah,+Irbid/@32.5629476,35.8411014,15z/data=!3m1!4b1!4m5!3m4!1s0x151c742020629675:0x78f48fb57817d43a!8m2!3d32.5632328!4d35.8499812?hl=en
', N'Al A''awdah
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (310, N'حي التل
', 3, 30, N'https://www.google.com/maps/place/Al+Tal,+Irbid/@32.5586212,35.8428079,16z/data=!3m1!4b1!4m5!3m4!1s0x151c76a19fb745e3:0xb508ebc976e2d269!8m2!3d32.557901!4d35.8476564?hl=en
', N'Al Tal
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (311, N'حي الجامع
', 3, 30, N'https://www.google.com/maps/place/Al+Jama''ah,+Irbid/@32.5431638,35.8382401,14z/data=!4m5!3m4!1s0x151c76938f069ffd:0x546eff706f46929f!8m2!3d32.5447422!4d35.8539344?hl=en
', N'Al Jama''ah
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (312, N'حي الهاشمي
', 3, 30, N'https://www.google.com/maps/place/Al+Hashmi,+Irbid/@32.5560844,35.8519113,16z/data=!4m5!3m4!1s0x151c769f05f0356d:0xc636ca20c8938066!8m2!3d32.55687!4d35.8519171?hl=en
', N'Al Hashmi
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (313, N'حي الملعب
', 3, 30, N'https://www.google.com/maps/place/Al+Mala''ab,+Irbid/@32.5536723,35.8543399,16z/data=!3m1!4b1!4m5!3m4!1s0x151c769b62ba27b3:0xef9d8c4459913617!8m2!3d32.5543861!4d35.860199?hl=en
', N'Al Mala''ab
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (314, N'حي الاندلس
', 3, 30, N'https://www.google.com/maps/place/Al+Andalus,+Irbid/@32.5554902,35.8550952,14z/data=!3m1!4b1!4m5!3m4!1s0x151c75d538d12b47:0x4dae60e8eb53048a!8m2!3d32.5544401!4d35.8718945?hl=en
', N'Al Andalus
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (315, N'حي الزهراء
', 3, 30, N'https://www.google.com/maps/place/Al+Zahra'',+Irbid/@32.5479247,35.8547185,14z/data=!3m1!4b1!4m5!3m4!1s0x151c767d8d9ebb1b:0xb438c1820e78ea09!8m2!3d32.5485093!4d35.8756309?hl=en
', N'Al Zahra''
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (316, N'حي الورود
', 3, 30, N'https://www.google.com/maps/place/Al+Worod,+Irbid/@32.5424712,35.856612,14z/data=!3m1!4b1!4m5!3m4!1s0x151c7662e0ed4885:0x4d4ab38322aa61e8!8m2!3d32.5407099!4d35.8689355?hl=en
', N'Al Worod
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (317, N'حي الحكمة
', 3, 30, N'https://www.google.com/maps/place/Al+Hikmah,+Irbid/@32.5436996,35.8536905,15z/data=!3m1!4b1!4m5!3m4!1s0x151c768e6964fb6d:0x390473eabb42a6be!8m2!3d32.5436608!4d35.8626822?hl=en
', N'Al Hikmah
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (318, N'حي النزهة
', 3, 30, N'https://www.google.com/maps/place/Al+Nozha,+Irbid/@32.5487913,35.8462747,15z/data=!3m1!4b1!4m5!3m4!1s0x151c76973589e3f9:0xb8d6e9637e37a6!8m2!3d32.55103!4d35.855957?hl=en
', N'Al Nozha
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (319, N'حي الجامعة
', 3, 30, N'https://www.google.com/maps/place/Al+Jama''ah,+Irbid/@32.5431638,35.8382401,14z/data=!3m1!4b1!4m5!3m4!1s0x151c76938f069ffd:0x546eff706f46929f!8m2!3d32.5447422!4d35.8539344?hl=en
', N'Al Jama''ah
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (320, N'حي النظيف
', 3, 30, N'https://www.google.com/maps/place/Al+Nadif,+Irbid/@32.5549396,35.8316191,15z/data=!3m1!4b1!4m5!3m4!1s0x151c76a53511fbeb:0x6ba5adfbb5a12371!8m2!3d32.5545725!4d35.8399928?hl=en
', N'Al Nadif
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (321, N'حي المنارة
', 3, 30, N'https://www.google.com/maps/place/Al+Manara,+Irbid/@32.55566,35.8219695,15z/data=!3m1!4b1!4m5!3m4!1s0x151c76b2557e041f:0x475f6de4f9041547!8m2!3d32.5562397!4d35.8310158?hl=en
', N'Al Manara
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (322, N'حي الفضيلة
', 3, 30, N'https://www.google.com/maps/place/Al+Fadilah,+Irbid/@32.5464445,35.8288975,15z/data=!3m1!4b1!4m5!3m4!1s0x151c76b7e7c4cb91:0xed7f22e80d4d5314!8m2!3d32.5474671!4d35.835975?hl=en
', N'Al Fadilah
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (323, N'حي الابرار
', 3, 30, N'https://www.google.com/maps/place/Al+Abrar,+Irbid/@32.5491754,35.8352296,15z/data=!3m1!4b1!4m5!3m4!1s0x151c76bbfbd32275:0x6f1ada3494ae50a4!8m2!3d32.5492531!4d35.8453031?hl=en
', N'Al Abrar
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (324, N'حي الافراح
', 3, 30, N'https://www.google.com/maps/place/Al+Afrah,+Irbid/@32.5348109,35.8393306,15z/data=!3m1!4b1!4m5!3m4!1s0x151c76c2f6a6baa9:0x51efeef5d0ce7762!8m2!3d32.5352708!4d35.8486765?hl=en
', N'Al Afrah
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (325, N'حي الاطباء
', 3, 30, N'https://www.google.com/maps/place/Al+Atiba'',+Irbid/@32.5308732,35.831798,15z/data=!3m1!4b1!4m5!3m4!1s0x151c76da3c91d489:0x923044dc552c7832!8m2!3d32.5320928!4d35.8404833?hl=en
', N'Al Atiba''
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (326, N'حي جديد
', 3, 30, N'https://www.google.com/maps/place/Jadeed,+Irbid/@32.5308732,35.831798,15z/data=!4m5!3m4!1s0x151c76cdfde7c1ab:0x6efe96834543284e!8m2!3d32.5393613!4d35.8327639?hl=en
', N'Jadeed
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (327, N'بيت المقدس
', 3, 31, N'https://www.google.com/maps/place/Bayt+Al+Maqdes,+Madaba/@31.7214767,35.7823698,14z/data=!3m1!4b1!4m5!3m4!1s0x151cacc43d087f51:0xe32f2175bd16f8d1!8m2!3d31.7196764!4d35.79904?hl=en
', N'Bayt Al Maqdes
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (328, N'الهاشمي
', 3, 31, N'https://www.google.com/maps/place/Al+Hashimi,+Madaba/@31.7157134,35.7873417,15z/data=!3m1!4b1!4m5!3m4!1s0x151cacce3b7129ef:0x8998bfaa2c994fc2!8m2!3d31.7155929!4d35.7947424?hl=en
', N'Al Hashimi
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (329, N'الحمد
', 3, 31, N'https://www.google.com/maps/place/Al+Hamd,+Madaba/@31.7212033,35.7918472,14z/data=!3m1!4b1!4m5!3m4!1s0x151cacbee6e5c3ef:0x3706729b678e052d!8m2!3d31.7187451!4d35.8037958?hl=en
', N'Al Hamd
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (330, N'الزيتونة
', 3, 31, N'https://www.google.com/maps/place/Al+Zaytooneh,+Madaba/@31.7270915,35.8037119,15z/data=!3m1!4b1!4m5!3m4!1s0x151cac9690d739fb:0x298c425496e40d9a!8m2!3d31.7259433!4d35.812768?hl=en
', N'Al Zaytooneh
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (331, N'حنينا
', 3, 31, N'https://www.google.com/maps/place/Hanina,+Madaba/@31.7289288,35.7873537,14z/data=!3m1!4b1!4m5!3m4!1s0x151caceb21efa483:0x1b15a71698affbb5!8m2!3d31.7292645!4d35.8052321?hl=en
', N'Hanina
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (332, N'الشفاء
', 3, 31, N'https://www.google.com/maps/place/Al+Shifa''a,+Madaba/@31.7313961,35.788068,15z/data=!3m1!4b1!4m5!3m4!1s0x151cace8f5acaa6b:0x94a35d8a28e88a8!8m2!3d31.7318429!4d35.7968476?hl=en
', N'Al Shifa''a
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (333, N'مؤتة
', 3, 31, N'https://www.google.com/maps/place/Mo''tah,+Madaba/@31.7101146,35.7881337,16z/data=!3m1!4b1!4m5!3m4!1s0x1503532cd585b71f:0xc78b94bd75e4ce99!8m2!3d31.7103596!4d35.7917904?hl=en
', N'Mo''tah
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (334, N'التنمية
', 3, 31, N'https://www.google.com/maps/place/Al+Tanmiyah,+Madaba/@31.7369181,35.7857812,14z/data=!3m1!4b1!4m5!3m4!1s0x151cacf27e7f750d:0xdd93884315a4d2a9!8m2!3d31.7370762!4d35.7990551?hl=en
', N'Al Tanmiyah
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (335, N'النصر
', 3, 31, N'https://www.google.com/maps/place/Al+Nasser,+Madaba/@31.733627,35.778188,15z/data=!3m1!4b1!4m5!3m4!1s0x151cace058fbb209:0xb509dfd11c02664c!8m2!3d31.734874!4d35.7852886?hl=en
', N'Al Nasser
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (336, N'الفيحاء
', 3, 31, N'https://www.google.com/maps/place/Al+Faiha''a,+Madaba/@31.7139836,35.7732961,15z/data=!3m1!4b1!4m5!3m4!1s0x151cad2ad3e0799f:0xe03eec6aa164b635!8m2!3d31.7158969!4d35.7799218?hl=en
', N'Al Faiha''a
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (337, N'محمد الفاتح
', 3, 31, N'https://www.google.com/maps/place/Muhammad+Al+Fateh,+Madaba/@31.7234792,35.7803138,15z/data=!3m1!4b1!4m5!3m4!1s0x151cacd99b5b6955:0xc6d134c04cabd34d!8m2!3d31.7229869!4d35.7900583?hl=en
', N'Muhammad Al Fateh
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (338, N'الخلفاء
', 3, 31, N'https://www.google.com/maps/place/Al+Kholafa''a,+Madaba/@31.7211938,35.7675714,14z/data=!3m1!4b1!4m5!3m4!1s0x151cacd7c33a0fc1:0x309855063db512d9!8m2!3d31.7197616!4d35.7862082?hl=en
', N'Al Kholafa''a
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (339, N'الأندلس
', 3, 31, N'https://www.google.com/maps/place/Al+Andalus,+Madaba/@31.705269,35.7851568,16z/data=!3m1!4b1!4m5!3m4!1s0x150353294bc6a64d:0xaa633b98dd5f3269!8m2!3d31.7052355!4d35.789929?hl=en
', N'Al Andalus
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (340, N'المخيم
', 3, 31, N'https://www.google.com/maps/place/Al+Mokhayam,+Madaba/@31.7118594,35.7802706,15z/data=!3m1!4b1!4m5!3m4!1s0x151cacd4b1b9e26f:0xfe0ae3962308413f!8m2!3d31.7119534!4d35.7877897?hl=en
', N'Al Mokhayam
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (341, N'العلماء
', 3, 31, N'https://www.google.com/maps/place/Al+Ulama''a,+Madaba/@31.7058434,35.7662033,14z/data=!3m1!4b1!4m5!3m4!1s0x150352d62d7a9b93:0x6a02ac12dd700ef0!8m2!3d31.7071589!4d35.7829898?hl=en
', N'Al Ulama''a
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (342, N'مجمع السفريات
', 3, 31, N'https://www.google.com/maps/place/Mojamma''+Al+Safariyat,+Madaba/@31.7130918,35.7907646,15z/data=!3m1!4b1!4m5!3m4!1s0x151caccea05fcbf5:0x9ec4a6d9a4f47c2!8m2!3d31.7136118!4d35.7997038?hl=en
', N'Mojamma'' Al Safariyat
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (343, N'الكرامة
', 3, 31, N'https://www.google.com/maps/place/Al+Karameh,+Madaba/@31.7113908,35.7965643,15z/data=!3m1!4b1!4m5!3m4!1s0x151cacc9f6df3405:0x92e8f49c46b80b15!8m2!3d31.7127738!4d35.8032258?hl=en
', N'Al Karameh
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (344, N'الزهور
', 3, 31, N'https://www.google.com/maps/place/Al+Zohour,+Madaba/@31.7096858,35.8025858,15z/data=!3m1!4b1!4m5!3m4!1s0x151cacb6adce6b75:0x5a6f82f1fd39c274!8m2!3d31.7110604!4d35.8112167?hl=en
', N'Al Zohour
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (345, N'السعادة
', 3, 31, N'https://www.google.com/maps/place/Al+Sa''adeh,+Madaba/@31.7070169,35.8068359,15z/data=!3m1!4b1!4m5!3m4!1s0x151cacb23dafc019:0x2d0eea094f5c070b!8m2!3d31.7032027!4d35.8139196?hl=en
', N'Al Sa''adeh
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (346, N'النزهة
', 3, 31, N'https://www.google.com/maps/place/Al+Nnozha,+Madaba/@31.6974293,35.7884065,14z/data=!3m1!4b1!4m5!3m4!1s0x15035337de684b19:0x40ce865c95dfafb3!8m2!3d31.6979192!4d35.8061194?hl=en
', N'Al Nnozha
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (347, N'الجزيرة
', 3, 31, N'https://www.google.com/maps/place/Al+Jazira,+Madaba/@31.7009364,35.7904111,15z/data=!3m1!4b1!4m5!3m4!1s0x150353311ee6ab8b:0x9d177c675a229a3c!8m2!3d31.7049999!4d35.8028306?hl=en
', N'Al Jazira
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (348, N'أبو عبيدة
', 3, 31, N'https://www.google.com/maps/place/Abu+Obaydah,+Madaba/@31.7034577,35.7874058,15z/data=!3m1!4b1!4m5!3m4!1s0x15035331ee48a683:0xdb990a07715996d!8m2!3d31.7093016!4d35.7963304?hl=en
', N'Abu Obaydah
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (349, N'وادي الحلبي
', 3, 32, N'https://www.google.com/maps/place/Wadi+Al+Halabi,+As-Salt/@32.0468516,35.7178666,16z/data=!3m1!4b1!4m5!3m4!1s0x151cbd5ef1deea83:0xd17450eb03594e4e!8m2!3d32.0447512!4d35.7228875?hl=en
', N'Wadi Al Halabi
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (350, N'الميدان
', 3, 32, N'https://www.google.com/maps/place/Al+Maidan,+As-Salt/@32.0425376,35.7228718,16z/data=!3m1!4b1!4m5!3m4!1s0x151cbd5a3451d09d:0x167719331069b94d!8m2!3d32.0433964!4d35.7273643?hl=en
', N'Al Maidan
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (351, N'القلعة
', 3, 32, N'https://www.google.com/maps/place/Al+Qala''a,+As-Salt/@32.0416504,35.715222,15z/data=!3m1!4b1!4m5!3m4!1s0x151cbd5c8360f5fb:0x4770a19c32663d3d!8m2!3d32.0417538!4d35.7263107?hl=en
', N'Al Qala''a
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (352, N'وادي الأكراد
', 3, 32, N'https://www.google.com/maps/place/Wadi+Al+Akrad,+As-Salt/@32.0426015,35.7113012,15z/data=!3m1!4b1!4m5!3m4!1s0x151cbd5d6c8b36e9:0x5bf9da6af0280307!8m2!3d32.0408504!4d35.7212105?hl=en
', N'Wadi Al Akrad
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (353, N'الجدعة
', 3, 32, N'https://www.google.com/maps/place/Al+Jada''a,+As-Salt/@32.0368348,35.7219846,16z/data=!3m1!4b1!4m5!3m4!1s0x151cbd44fa7530c1:0x76b528345767f67d!8m2!3d32.0365277!4d35.7284655?hl=en', N'Al Jada''a
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (354, N'العيزرية
', 3, 32, N'https://www.google.com/maps/place/Al+Aizarieh,+As-Salt/@32.0361709,35.7128221,15z/data=!3m1!4b1!4m5!3m4!1s0x151cbd4232ab4ccd:0x62a70071097017fb!8m2!3d32.0361872!4d35.7234317?hl=en
', N'Al Aizarieh
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (355, N'المنشية
', 3, 32, N'https://www.google.com/maps/place/Al+Manshieh,+As-Salt/@32.0328406,35.7186644,15z/data=!3m1!4b1!4m5!3m4!1s0x151cbd46647acf6b:0xac862ba12c3fe345!8m2!3d32.0342045!4d35.7283703?hl=en
', N'Al Manshieh
')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (357, N'All', 1, NULL, NULL, N'All')
GO
INSERT [dbo].[Locations] ([Id], [LocationName], [LevelId], [ParentId], [GoogleURL], [LocationNameEn]) VALUES (359, N'All', 2, 357, NULL, N'All')
GO
SET IDENTITY_INSERT [dbo].[Locations] OFF
GO
