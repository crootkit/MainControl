from http.server import BaseHTTPRequestHandler, HTTPServer

class RequestHandler(BaseHTTPRequestHandler):
    def do_POST(self):
        # 获取 POST 请求的内容长度
        content_length = int(self.headers['Content-Length'])
        # 读取 POST 请求的内容
        body = self.rfile.read(content_length)
        # 将内容记录到文件中
        with open("request.log", "w") as f:
            f.write(body.decode("gbk"))
        # 发送响应
        self.send_response(200)
        self.end_headers()

# 创建服务器，绑定 80 端口
httpd = HTTPServer(('', 80), RequestHandler)
# 开始监听
print("start listening 80 ...")
httpd.serve_forever()

