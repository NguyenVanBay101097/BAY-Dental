import { Component, OnInit, Input, ViewChild, ViewContainerRef, ComponentFactoryResolver, SimpleChanges, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-audience-filter-dropdown',
  templateUrl: './audience-filter-dropdown.component.html',
  styleUrls: ['./audience-filter-dropdown.component.css']
})
export class AudienceFilterDropdownComponent implements OnInit {

  @Input() audience_filter_comp_data: { component, data }; // component and data 
  @Output() statusItem = new EventEmitter<any>();

  @ViewChild('container', {
    read: ViewContainerRef,
    static: true
  }) container: ViewContainerRef;

  constructor(private componentFactoryResolver: ComponentFactoryResolver) { }

  ngOnInit() {
    this.renderComponent();
  }

  ngOnChanges(changes: SimpleChanges) {
    this.renderComponent();
  }

  renderComponent() {
    const componentFactory = this.componentFactoryResolver.resolveComponentFactory(this.audience_filter_comp_data.component);

    const viewContainerRef = this.container;
    viewContainerRef.clear();

    const componentRef = viewContainerRef.createComponent(componentFactory);
    (<any>componentRef.instance).dataReceive = this.audience_filter_comp_data.data;
    (<any>componentRef.instance).dataSend.subscribe(res => this.statusItem.emit(res));
  }
}
