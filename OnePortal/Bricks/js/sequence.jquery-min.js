
/*
Sequence.js (http://www.sequencejs.com)
Version: 0.8 Beta
Author: Ian Lunn @IanLunn
Author URL: http://www.ianlunn.co.uk/
Github: https://github.com/IanLunn/Sequence

This is a FREE script and is available under a MIT License:
http://www.opensource.org/licenses/mit-license.php

Sequence.js and its dependencies are (c) Ian Lunn Design 2012 unless otherwise stated.

Sequence also relies on the following open source scripts:

-   jQuery imagesLoaded 2.1.0 (http://github.com/desandro/imagesloaded)
    Paul Irish et al 
    Available under a MIT License: http://www.opensource.org/licenses/mit-license.php

-   jQuery TouchWipe 1.1.1 (http://www.netcu.de/jquery-touchwipe-iphone-ipad-library)
    Andreas Waltl, netCU Internetagentur (http://www.netcu.de)
    Available under a MIT License: http://www.opensource.org/licenses/mit-license.php

-   Modernizr 2.6.1 Custom Build (http://modernizr.com/)
    Copyright (c) Faruk Ates, Paul Irish, Alex Sexton
    Available under the BSD and MIT licenses: www.modernizr.com/license/
*/(function(e){function t(t,n,r,i){function f(){s.afterLoaded();s.settings.hideFramesUntilPreloaded&&s.settings.preloader&&s.sequence.children("li").show();if(s.settings.preloader)if(s.settings.hidePreloaderUsingCSS&&s.transitionsSupported){s.prependPreloadingCompleteTo=s.settings.prependPreloadingComplete==1?s.settings.preloader:e(s.settings.prependPreloadingComplete);s.prependPreloadingCompleteTo.addClass("preloading-complete");setTimeout(y,s.settings.hidePreloaderDelay)}else s.settings.preloader.fadeOut(s.settings.hidePreloaderDelay,function(){clearInterval(s.defaultPreloader);y()});else y()}function g(t,n){function l(){var t=e(a),s=e(f);i&&(f.length?i.reject(o,t,s):i.resolve(o));e.isFunction(n)&&n.call(r,o,t,s)}function c(t,n){if(t.src===BLANK||e.inArray(t,u)!==-1)return;u.push(t);n?f.push(t):a.push(t);e.data(t,"imagesLoaded",{isBroken:n,src:t.src});s&&i.notifyWith(e(t),[n,o,e(a),e(f)]);if(o.length===u.length){setTimeout(l);o.unbind(".imagesLoaded")}}BLANK="data:image/gif;base64,R0lGODlhAQABAIAAAAAAAP///ywAAAAAAQABAAACAUwAOw==";var r=t,i=e.isFunction(e.Deferred)?e.Deferred():0,s=e.isFunction(i.notify),o=r.find("img").add(r.filter("img")),u=[],a=[],f=[];e.isPlainObject(n)&&e.each(n,function(e,t){e==="callback"?n=t:i&&i[e](t)});o.length?o.bind("load.imagesLoaded error.imagesLoaded",function(e){c(e.target,e.type==="error")}).each(function(t,n){var r=n.src,i=e.data(n,"imagesLoaded");if(i&&i.src===r){c(n,i.isBroken);return}if(n.complete&&n.naturalWidth!==undefined){c(n,n.naturalWidth===0||n.naturalHeight===0);return}if(n.readyState||n.complete){n.src=BLANK;n.src=r}}):l()}function y(){e(s.settings.preloader).remove();s.nextButton=s.init.uiElements(s.settings.nextButton,".next");s.prevButton=s.init.uiElements(s.settings.prevButton,".prev");s.pauseButton=s.init.uiElements(s.settings.pauseButton,".pause");s.nextButton!==undefined&&s.nextButton!==!1&&s.settings.showNextButtonOnInit&&s.nextButton.show();s.prevButton!==undefined&&s.prevButton!==!1&&s.settings.showPrevButtonOnInit&&s.prevButton.show();s.pauseButton!==undefined&&s.pauseButton!==!1&&s.pauseButton.show();if(s.settings.pauseIcon!==!1){s.pauseIcon=s.init.uiElements(s.settings.pauseIcon,".pause-icon");s.pauseIcon!==undefined&&s.pauseIcon.hide()}else s.pauseIcon=undefined;s.nextFrameID=s.settings.startingFrameID;if(s.settings.hashTags){s.sequence.children("li").each(function(){s.frameHashID.push(e(this).attr(s.getHashTagFrom))});s.currentHashTag=location.hash.replace("#","");if(s.currentHashTag===undefined||s.currentHashTag==="")s.nextFrameID=s.settings.startingFrameID;else{s.frameHashIndex=e.inArray(s.currentHashTag,s.frameHashID);s.frameHashIndex!==-1?s.nextFrameID=s.frameHashIndex+1:s.nextFrameID=s.settings.startingFrameID}}s.nextFrame=s.sequence.children("li:nth-child("+s.nextFrameID+")");s.nextFrameChildren=s.nextFrame.children();s.sequence.css({width:"100%",height:"100%",position:"relative"});s.sequence.children("li").css({width:"100%",height:"100%",position:"absolute"});if(s.transitionsSupported)if(!s.settings.animateStartingFrameIn){s.currentFrameID=s.nextFrameID;s.settings.moveActiveFrameToTop&&s.nextFrame.css("z-index",s.numberOfFrames);s.modifyElements(s.nextFrameChildren,"0s");s.nextFrame.addClass("animate-in");if(s.settings.hashChangesOnFirstFrame){s.currentHashTag=s.nextFrame.attr(s.getHashTagFrom);document.location.hash="#"+s.currentHashTag}setTimeout(function(){s.modifyElements(s.nextFrameChildren,"")},100);s.startAutoPlay(s.settings.autoPlayDelay)}else if(s.settings.reverseAnimationsWhenNavigatingBackwards&&s.settings.autoPlayDirection-1&&s.settings.animateStartingFrameIn){s.modifyElements(s.nextFrameChildren,"0s");s.nextFrame.addClass("animate-out");s.goTo(s.nextFrameID,-1)}else s.goTo(s.nextFrameID,1);else{s.container.addClass("sequence-fallback");s.beforeNextFrameAnimatesIn();s.afterNextFrameAnimatesIn();if(s.settings.hashChangesOnFirstFrame){s.currentHashTag=s.nextFrame.attr(s.getHashTagFrom);document.location.hash="#"+s.currentHashTag}s.currentFrameID=s.nextFrameID;s.sequence.children("li").addClass("animate-in");s.sequence.children(":not(li:nth-child("+s.nextFrameID+"))").css({display:"none",opacity:0});s.startAutoPlay(s.settings.autoPlayDelay)}s.nextButton!==undefined&&s.nextButton.click(function(){s.next()});s.prevButton!==undefined&&s.prevButton.click(function(){s.prev()});s.pauseButton!==undefined&&s.pauseButton.click(function(){s.pause(!0)});if(s.settings.keyNavigation){var t={left:37,right:39};function n(e,n){var r;for(keyCodes in n){keyCodes==="left"||keyCodes==="right"?r=t[keyCodes]:r=keyCodes;e===parseFloat(r)&&s.initCustomKeyEvent(n[keyCodes])}}e(document).keydown(function(e){var t=String.fromCharCode(e.keyCode);if(t>0&&t<=s.numberOfFrames&&s.settings.numericKeysGoToFrames){s.nextFrameID=t;s.goTo(s.nextFrameID)}n(e.keyCode,s.settings.keyEvents);n(e.keyCode,s.settings.customKeyEvents)})}s.settings.pauseOnHover&&s.settings.autoPlay&&!s.hasTouch&&s.sequence.on({mouseenter:function(){s.mouseover=!0;s.isHardPaused||s.pause()},mouseleave:function(){s.mouseover=!1;s.isHardPaused||s.unpause()}});s.settings.hashTags&&e(window).hashchange(function(){newTag=location.hash.replace("#","");if(s.currentHashTag!==newTag){s.currentHashTag=newTag;s.frameHashIndex=e.inArray(s.currentHashTag,s.frameHashID);if(s.frameHashIndex!==-1){s.nextFrameID=s.frameHashIndex+1;s.goTo(s.nextFrameID)}}});if(s.settings.swipeNavigation&&s.hasTouch){var r,i,o=!1;function u(){s.sequence.on("touchmove",a);r=null;o=!1}function a(e){s.settings.swipePreventsDefault&&e.preventDefault();if(o){var t=e.originalEvent.touches[0].pageX,n=e.originalEvent.touches[0].pageY,a=r-t,f=i-n;if(Math.abs(a)>=s.settings.swipeThreshold){u();a>0?s.initCustomKeyEvent(s.settings.swipeEvents.left):s.initCustomKeyEvent(s.settings.swipeEvents.right)}else if(Math.abs(f)>=s.settings.swipeThreshold){u();f>0?s.initCustomKeyEvent(s.settings.swipeEvents.down):s.initCustomKeyEvent(s.settings.swipeEvents.up)}}}function f(e){if(e.originalEvent.touches.length==1){r=e.originalEvent.touches[0].pageX;i=e.originalEvent.touches[0].pageY;o=!0;s.sequence.on("touchmove",a)}}s.sequence.on("touchstart",f)}}var s=this;s.container=e(t),s.sequence=s.container.children("ul");try{Modernizr.prefixed;if(Modernizr.prefixed===undefined)throw"undefined"}catch(o){i.modernizr()}var u={WebkitTransition:"-webkit-",MozTransition:"-moz-",OTransition:"-o-",msTransition:"-ms-",transition:""},a={WebkitTransition:"webkitTransitionEnd webkitAnimationEnd",MozTransition:"transitionend animationend",OTransition:"otransitionend oanimationend",msTransition:"MSTransitionEnd MSAnimationEnd",transition:"transitionend animationend"};s.prefix=u[Modernizr.prefixed("transition")],s.transitionEnd=a[Modernizr.prefixed("transition")],s.transitionProperties={},s.numberOfFrames=s.sequence.children("li").length,s.transitionsSupported=s.prefix!==undefined?!0:!1,s.hasTouch="ontouchstart"in window?!0:!1,s.active,s.navigationSkipThresholdActive=!1,s.autoPlayTimer,s.isPaused=!1,s.isHardPaused=!1,s.mouseover=!1,s.defaultPreloader,s.nextButton,s.prevButton,s.pauseButton,s.pauseIcon,s.delayUnpause,s.init={uiElements:function(t,n){switch(t){case!1:return undefined;case!0:n===".sequence-preloader"&&i.defaultPreloader(s.container,s.transitionsSupported,s.prefix);return e(n);default:return e(t)}}};s.paused=function(){},s.unpaused=function(){},s.beforeNextFrameAnimatesIn=function(){},s.afterNextFrameAnimatesIn=function(){},s.beforeCurrentFrameAnimatesOut=function(){},s.afterCurrentFrameAnimatesOut=function(){},s.beforeFirstFrameAnimatesIn=function(){},s.afterFirstFrameAnimatesIn=function(){},s.beforeLastFrameAnimatesIn=function(){},s.afterLastFrameAnimatesIn=function(){},s.afterLoaded=function(){};s.settings=e.extend({},r,n);s.settings.preloader=s.init.uiElements(s.settings.preloader,".sequence-preloader");s.firstFrame=s.settings.animateStartingFrameIn?!0:!1;s.settings.unpauseDelay=s.settings.unpauseDelay===null?s.settings.autoPlayDelay:s.settings.unpauseDelay;s.currentHashTag;s.getHashTagFrom=s.settings.hashDataAttribute?"data-sequence-hashtag":"id";s.frameHashID=[];s.direction=s.settings.autoPlayDirection;s.settings.hideFramesUntilPreloaded&&s.settings.preloader&&s.sequence.children("li").hide();s.prefix==="-o-"&&(s.transitionsSupported=i.operaTest());s.modifyElements(s.sequence.children("li"),"0s");s.sequence.children("li").removeClass("animate-in");var l=s.settings.preloadTheseFrames.length,c=s.settings.preloadTheseImages.length;if(!s.settings.preloader||l===0&&c===0)e(window).bind("load",function(){f();e(this).unbind("load")});else{function h(t,n){var r=[];if(!n)for(var i=t;i>0;i--)s.sequence.children("li:nth-child("+s.settings.preloadTheseFrames[i-1]+")").find("img").each(function(){r.push(e(this)[0])});else for(var i=t;i>0;i--)r.push(e("body").find('img[src="'+s.settings.preloadTheseImages[i-1]+'"]')[0]);return r}var p=h(l),d=h(c,!0),v=e(p.concat(d)),m=v.length;g(v,f)}}t.prototype={initCustomKeyEvent:function(e){var t=this;switch(e){case"next":t.next();break;case"prev":t.prev();break;case"pause":t.pause(!0)}},modifyElements:function(e,t){var n=this;e.css(n.prefixCSS(n.prefix,{"transition-duration":t,"transition-delay":t}))},prefixCSS:function(e,t){var n={};for(property in t)n[e+property]=t[property];return n},setTransitionProperties:function(t){var n=this;t.each(function(){n.transitionProperties["transition-duration"]=e(this).css(n.prefix+"transition-duration");n.transitionProperties["transition-delay"]=e(this).css(n.prefix+"transition-delay");e(this).css(n.prefixCSS(n.prefix,n.transitionProperties))})},startAutoPlay:function(e){var t=this;if(t.settings.autoPlay&&!t.isPaused){t.stopAutoPlay();t.autoPlayTimer=setTimeout(function(){t.settings.autoPlayDirection===1?t.next():t.prev()},e)}},stopAutoPlay:function(){var e=this;clearTimeout(e.autoPlayTimer)},pause:function(e){var t=this;if(!t.isPaused){if(t.pauseButton!==undefined){t.pauseButton.addClass("paused");t.pauseIcon!==undefined&&t.pauseIcon.show()}t.paused();t.isPaused=!0;t.isHardPaused=e?!0:!1;t.stopAutoPlay()}else t.unpause()},unpause:function(e){var t=this;if(t.pauseButton!==undefined){t.pauseButton.removeClass("paused");t.pauseIcon!==undefined&&t.pauseIcon.hide()}t.isPaused=!1;t.isHardPaused=!1;if(!t.active){e!==!1&&t.unpaused();t.startAutoPlay(t.settings.unpauseDelay)}else t.delayUnpause=!0},next:function(){var e=this;e.nextFrameID=e.currentFrameID!==e.numberOfFrames?e.currentFrameID+1:1;e.goTo(e.nextFrameID,1)},prev:function(){var e=this;e.nextFrameID=e.currentFrameID===1?e.numberOfFrames:e.currentFrameID-1;e.goTo(e.nextFrameID,-1)},goTo:function(e,t){var n=this,e=parseFloat(e);if(e===n.currentFrameID||n.settings.navigationSkip&&n.navigationSkipThresholdActive||!n.settings.navigationSkip&&n.active||!n.transitionsSupported&&n.active||!n.settings.cycle&&t===1&&n.currentFrameID===n.numberOfFrames||!n.settings.cycle&&t===-1&&n.currentFrameID===1||n.settings.preventReverseSkipping&&n.direction!==t&&n.active)return!1;if(n.settings.navigationSkip&&n.active){n.navigationSkipThresholdActive=!0;n.settings.fadeFrameWhenSkipped&&n.nextFrame.stop().animate({opacity:0},n.settings.fadeFrameTime);navigationSkipThresholdTimer=setTimeout(function(){n.navigationSkipThresholdActive=!1},n.settings.navigationSkipThreshold)}if(!n.active||n.settings.navigationSkip){n.active=!0;n.stopAutoPlay();e===n.numberOfFrames?n.beforeLastFrameAnimatesIn():e===1&&n.beforeFirstFrameAnimatesIn();t===undefined?n.direction=e>n.currentFrameID?1:-1:n.direction=t;n.currentFrame=n.sequence.children(".animate-in");n.nextFrame=n.sequence.children("li:nth-child("+e+")");n.frameChildren=n.currentFrame.children();n.nextFrameChildren=n.nextFrame.children();if(n.transitionsSupported){if(!n.firstFrame){if(n.currentFrame.length!==undefined){n.beforeCurrentFrameAnimatesOut();n.settings.moveActiveFrameToTop&&n.currentFrame.css("z-index",1);n.modifyElements(n.nextFrameChildren,"0s");if(!n.settings.reverseAnimationsWhenNavigatingBackwards||n.direction===1){n.nextFrame.removeClass("animate-out");n.modifyElements(n.frameChildren,"")}else if(n.settings.reverseAnimationsWhenNavigatingBackwards&&n.direction===-1){n.nextFrame.addClass("animate-out");n.setTransitionProperties(n.frameChildren)}}}else n.firstFrame=!1;n.active=!0;n.currentFrame.unbind(n.transitionEnd);n.nextFrame.unbind(n.transitionEnd);n.settings.fadeFrameWhenSkipped&&n.nextFrame.css("opacity",1);n.beforeNextFrameAnimatesIn();n.settings.moveActiveFrameToTop&&n.nextFrame.css({"z-index":n.numberOfFrames});!n.settings.reverseAnimationsWhenNavigatingBackwards||n.direction===1?setTimeout(function(){n.modifyElements(n.nextFrameChildren,"");n.waitForAnimationsToComplete(n.nextFrame,n.nextFrameChildren,"in");n.afterCurrentFrameAnimatesOut!=="function () {}"&&n.waitForAnimationsToComplete(n.currentFrame,n.frameChildren,"out")},50):n.settings.reverseAnimationsWhenNavigatingBackwards&&n.direction===-1&&setTimeout(function(){n.modifyElements(n.nextFrameChildren,"");n.setTransitionProperties(n.frameChildren);n.waitForAnimationsToComplete(n.nextFrame,n.nextFrameChildren,"in");n.afterCurrentFrameAnimatesOut!="function () {}"&&n.waitForAnimationsToComplete(n.currentFrame,n.frameChildren,"out")},50);!n.settings.reverseAnimationsWhenNavigatingBackwards||n.direction===1?setTimeout(function(){n.currentFrame.toggleClass("animate-out animate-in");n.nextFrame.addClass("animate-in")},50):n.settings.reverseAnimationsWhenNavigatingBackwards&&n.direction===-1&&setTimeout(function(){n.nextFrame.toggleClass("animate-out animate-in");n.currentFrame.removeClass("animate-in")},50)}else{function r(){n.setHashTag();n.active=!1;n.startAutoPlay(n.settings.autoPlayDelay)}n.beforeCurrentFrameAnimatesOut();switch(n.settings.fallback.theme){case"fade":n.sequence.children("li").css({position:"relative"});n.currentFrame.animate({opacity:0},n.settings.fallback.speed,function(){n.currentFrame.css({display:"none","z-index":"1"});n.beforeNextFrameAnimatesIn();n.nextFrame.css({display:"block","z-index":n.numberOfFrames}).animate({opacity:1},500,function(){n.afterNextFrameAnimatesIn()});r()});n.sequence.children("li").css({position:"relative"});break;case"slide":default:var i={},s={},o={};if(n.direction===1){i.left="-100%";s.left="100%"}else{i.left="100%";s.left="-100%"}o.left="0";o.opacity=1;n.currentFrame=n.sequence.children("li:nth-child("+n.currentFrameID+")");n.currentFrame.animate(i,n.settings.fallback.speed);n.beforeNextFrameAnimatesIn();n.nextFrame.show().css(s);setTimeout(function(){n.nextFrame.animate(o,n.settings.fallback.speed,function(){r();n.afterNextFrameAnimatesIn()})},50)}}n.currentFrameID=e}},waitForAnimationsToComplete:function(t,n,r){var i=this;if(r==="out")var s=function(){i.afterCurrentFrameAnimatesOut()};else if(r==="in")var s=function(){i.afterNextFrameAnimatesIn();i.setHashTag();i.currentFrameID===i.numberOfFrames?i.afterLastFrameAnimatesIn():i.currentFrameID===1&&i.afterFirstFrameAnimatesIn();i.active=!1;if(!i.isHardPaused&&!i.mouseover)if(!i.delayUnpause)i.unpause(!1);else{i.delayUnpause=!1;i.unpause()}};n.data("animationEnded",!1);t.bind(i.transitionEnd,function(r){e(r.target).data("animationEnded",!0);var o=!0;n.each(function(){if(e(this).data("animationEnded")===!1){o=!1;return!1}});if(o){t.unbind(i.transitionEnd);s()}})},setHashTag:function(){var t=this;if(t.settings.hashTags){t.currentHashTag=t.currentFrame.attr(t.getHashTagFrom);t.frameHashIndex=e.inArray(t.currentHashTag,t.frameHashID);if(t.frameHashIndex!==-1&&(t.settings.hashChangesOnFirstFrame||!t.firstFrame)){t.nextFrameID=t.frameHashIndex+1;document.location.hash="#"+t.currentHashTag}else{t.nextFrameID=t.settings.startingFrameID;t.firstFrame=!1}}}};e.fn.sequence=function(i){var s=this;return s.each(function(){var s=new t(e(this),i,r,n);e(this).data("sequence",s)})};var n={modernizr:function(){window.Modernizr=function(e,t,n){function r(e){v.cssText=e}function i(e,t){return r(prefixes.join(e+";")+(t||""))}function s(e,t){return typeof e===t}function o(e,t){return!!~(""+e).indexOf(t)}function u(e,t){for(var r in e){var i=e[r];if(!o(i,"-")&&v[i]!==n)return t=="pfx"?i:!0}return!1}function a(e,t,r){for(var i in e){var o=t[e[i]];if(o!==n)return r===!1?e[i]:s(o,"function")?o.bind(r||t):o}return!1}function f(e,t,n){var r=e.charAt(0).toUpperCase()+e.slice(1),i=(e+" "+b.join(r+" ")+r).split(" ");return s(t,"string")||s(t,"undefined")?u(i,t):(i=(e+" "+w.join(r+" ")+r).split(" "),a(i,t,n))}var l="2.6.1",c={},h=t.documentElement,p="modernizr",d=t.createElement(p),v=d.style,m,g={}.toString,y="Webkit Moz O ms",b=y.split(" "),w=y.toLowerCase().split(" "),E={svg:"http://www.w3.org/2000/svg"},S={},x={},T={},N=[],C=N.slice,k,L={}.hasOwnProperty,A;!s(L,"undefined")&&!s(L.call,"undefined")?A=function(e,t){return L.call(e,t)}:A=function(e,t){return t in e&&s(e.constructor.prototype[t],"undefined")},Function.prototype.bind||(Function.prototype.bind=function(e){var t=self;if(typeof t!="function")throw new TypeError;var n=C.call(arguments,1),r=function(){if(self instanceof r){var i=function(){};i.prototype=t.prototype;var s=new i,o=t.apply(s,n.concat(C.call(arguments)));return Object(o)===o?o:s}return t.apply(e,n.concat(C.call(arguments)))};return r}),S.svg=function(){return!!t.createElementNS&&!!t.createElementNS(E.svg,"svg").createSVGRect};for(var O in S)A(S,O)&&(k=O.toLowerCase(),c[k]=S[O](),N.push((c[k]?"":"no-")+k));return c.addTest=function(e,t){if(typeof e=="object")for(var r in e)A(e,r)&&c.addTest(r,e[r]);else{e=e.toLowerCase();if(c[e]!==n)return c;t=typeof t=="function"?t():t,enableClasses&&(h.className+=" "+(t?"":"no-")+e),c[e]=t}return c},r(""),d=m=null,c._version=l,c._domPrefixes=w,c._cssomPrefixes=b,c.testProp=function(e){return u([e])},c.testAllProps=f,c.prefixed=function(e,t,n){return t?f(e,t,n):f(e,"pfx")},c}(self,self.document)},defaultPreloader:function(t,n,r){var i='<div class="sequence-preloader"><svg class="preloading" xmlns="http://www.w3.org/2000/svg"><circle class="circle" cx="6" cy="6" r="6" /><circle class="circle" cx="22" cy="6" r="6" /><circle class="circle" cx="38" cy="6" r="6" /></svg></div>';e("head").append("<style>.sequence-preloader{height: 100%;position: absolute;width: 100%;z-index: 999999;}@"+r+"keyframes preload{0%{opacity: 1;}50%{opacity: 0;}100%{opacity: 1;}}.sequence-preloader .preloading .circle{fill: #ff9442;display: inline-block;height: 12px;position: relative;top: -50%;width: 12px;"+r+"animation: preload 1s infinite; animation: preload 1s infinite;}.preloading{display:block;height: 12px;margin: 0 auto;top: 50%;margin-top:-6px;position: relative;width: 48px;}.sequence-preloader .preloading .circle:nth-child(2){"+r+"animation-delay: .15s; animation-delay: .15s;}.sequence-preloader .preloading .circle:nth-child(3){"+r+"animation-delay: .3s; animation-delay: .3s;}.preloading-complete{opacity: 0;visibility: hidden;"+r+"transition-duration: 1s; transition-duration: 1s;}div.inline{background-color: #ff9442; margin-right: 4px; float: left;}</style>");t.prepend(i);if(!Modernizr.svg&&!n){e(".sequence-preloader").prepend('<div class="preloading"><div class="circle inline"></div><div class="circle inline"></div><div class="circle inline"></div></div>');setInterval(function(){e(".sequence-preloader .circle").fadeToggle(500)},500)}else n||setInterval(function(){e(".sequence-preloader").fadeToggle(500)},500)},operaTest:function(){e("body").append('<span id="sequence-opera-test"></span>');var t=e("#sequence-opera-test");t.css("-o-transition","1s");return t.css("-o-transition")!="1s"?!1:!0}},r={startingFrameID:1,cycle:!0,animateStartingFrameIn:!1,reverseAnimationsWhenNavigatingBackwards:!0,moveActiveFrameToTop:!0,autoPlay:!0,autoPlayDirection:1,autoPlayDelay:5e3,navigationSkip:!0,navigationSkipThreshold:250,fadeFrameWhenSkipped:!0,fadeFrameTime:150,preventReverseSkipping:!1,nextButton:!1,showNextButtonOnInit:!0,prevButton:!1,showPrevButtonOnInit:!0,pauseButton:!1,unpauseDelay:null,pauseOnHover:!0,pauseIcon:!1,preloader:!1,preloadTheseFrames:[1],preloadTheseImages:[],hideFramesUntilPreloaded:!0,prependPreloadingComplete:!0,hidePreloaderUsingCSS:!0,hidePreloaderDelay:0,keyNavigation:!0,numericKeysGoToFrames:!0,keyEvents:{left:"prev",right:"next"},customKeyEvents:{},swipeNavigation:!0,swipeThreshold:20,swipePreventsDefault:!1,swipeEvents:{left:"prev",right:"next",up:!1,down:!1},hashTags:!1,hashDataAttribute:!1,hashChangesOnFirstFrame:!1,fallback:{theme:"slide",speed:500}}})(jQuery);