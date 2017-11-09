Svelto IoC container for Unity 3D
=====================================

Svelto IoC Container is the evolution of my first IoC container that I created as Proof of concept. Svelto IoC is extensively used in the project I am working on Robocraft (http://www.robocraftgame.com)

Please read my blog post for more information: http://www.sebaslab.com/svelto-inversion-of-control-container/

The ad hoc example can be found here: https://github.com/sebas77/Svelto.Ioc.Example

Note: since I wrote this project, I changed my mind about using an IoC Container. Nowadays I would use it only and exclusively as support tool for a GUI framework. However, without the hierarchical container feature, it wouldn't be useful for that either. I will add the hierarchical container feature in order to complete the project though. Today, I use mainly the Svelto.ECS pattern for my projects. An IoC container can lead to severe design issues, as I am going to write on a new article when I have time. The example is also quite old (I wrote it in 2012 and never updated it) and shows how awkward the code could get. In this case, the dependencies are mainly managers, which are way better handled by the Svelto ECS pattern. **If you still want to use an IoC container, please use ZenInject or Strange.IoC**

License
=====================================

Copyright (c) 2015 Sebastiano Mandalà

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
