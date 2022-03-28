# Alarm

Руководство git.

Установка программы
1) Через командную строку зайдите в папку, где хотите, чтобы хранился проект
2) git remote add origin https://github.com/Aniyar/Alarm.git
3) git pull origin master

Логин:
1) git config --global user.email "email"
2) git config --global user.name "username"

ПРОВЕРКА ПЕРЕД КОММИТОМ:
1) git status (Проверяет, на каком вы брэнче, какие файлы поменялись и т.д.)
2) git checkout 'branchname' (Если вы на чужой ветке)
  
Коммит:
1) git add 'filename' / git add . 
2) git commit -m "Сообщение"
3) git push origin 'branch'

Апдейт:
git pull origin master
  
Чтобы соединить свой бренч с основной версией, зайдите на гитхаб и нажмите на кнопку PULL REQUEST
