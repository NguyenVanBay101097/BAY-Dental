import { Component, OnInit, ViewChild, ComponentFactoryResolver } from '@angular/core';
import { SharedComponentDirective } from '../shared-component.directive';
import { SharedDemoDataDialogComponent } from '../shared-demo-data-dialog/shared-demo-data-dialog.component';

@Component({
  selector: 'app-shared-component',
  templateUrl: './shared-component.component.html',
  styleUrls: ['./shared-component.component.css']
})
export class SharedComponentComponent implements OnInit {
  @ViewChild(SharedComponentDirective, { static: false }) sharedHost: SharedComponentDirective
  constructor(
    private componentFactoryResolver: ComponentFactoryResolver,
  ) { }

  ngOnInit() {
  }

  loadComponent(adItem, position) {

    var viewContainerRef = this.sharedHost.viewContainerRef;
    const componentFactory = this.componentFactoryResolver.resolveComponentFactory(adItem.component);
    viewContainerRef.clear();
    var componentRef = viewContainerRef.createComponent(componentFactory);

    (<SharedDemoDataDialogComponent>componentRef.instance).productId = position.productId;

  }

}
