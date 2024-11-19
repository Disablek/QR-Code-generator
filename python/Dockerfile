FROM python

WORKDIR /QR-CODE-GENERATOR

COPY requirements.txt ./
RUN pip install --no-cache-dir -r requirements.txt

COPY . .

CMD [ "python", "./lok.py" ]
