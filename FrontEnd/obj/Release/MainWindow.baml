   M S B A M L   `   `   `  ���� O  Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem   FrontEnd   FrontEnd.MainWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     9System&clr-namespace:System;assembly=mscorlib       � Title$  QRCoder��$	��450��$	��800�� � 	Resources    Greeting  ButtonQRText�� %>   &	        &	      ��Hello, World!����	Click me!��.��� ���	#FFE9E9E9.+�9� -   $��Expander��$��Ещё��$
��Left=�$	��434��$��Center=�$	��115������ -    ��Name$ ContentBorder��$��0��._��� .+��� $��Содержимое 1��$��5q��� $��Содержимое 2��$��5q��� #���  $
��Left=�$��137,103,0,0q�$	��Top=�$��33��$��70���� $
��Left=�$	��328��$��
120,82,0,0q�
���   �$	��Top=�$	��463��
�������A� $
��Left=�$��58��$��555,216,0,0q�
���   �$	��Top=�$��58�����	#FFE9E9E9 J� RenderTransformOrigin$ 0.5,1&�$��10���� $
��Left=�$��60��$��584,216,0,0q�$	��Top=�$��30�����	#FFE9E9E9A� $
��Left=�$��58��$��556,216,0,0q�$	��Top=�$��56�����	#FFDADADA$ 0.5,1&�$��10�� � Triggers :�  :� RoutedEvent$ Expander.Expanded�� :� 
SourceName$ Expander��.[��� .���� .��W�  �� 
TargetName$ ContentBorder�� �� TargetProperty$ Height�	 W� From$	 0X�	
 W� To$	
 200X� g� Duration$ 0:0:0.5D�:� $ Expander.Collapsed��$ Expander��.[��� .���� .��W� $ ContentBorder��$ Height�$		 200X�$
 0X�$ 0:0:0.5D�