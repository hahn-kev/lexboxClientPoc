// @ts-ignore

import * as _ from 'lexbox-svelte';

console.log(_);

document.addEventListener('readystatechange', ev => {
  if(document.readyState === 'complete'){
      const text = document.createElement('p');
      text.innerText = 'Hello from SampleRazorClassLibraryWithJsAssets!';
      document.body.appendChild(text);
  }
});

