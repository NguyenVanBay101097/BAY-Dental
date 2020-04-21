import { Component, OnInit, ViewChild, ViewContainerRef, AfterViewInit, ComponentFactoryResolver, Input, Output, EventEmitter, SimpleChanges, OnChanges } from '@angular/core';
import { AudienceFilterItem } from '../facebook-mass-messaging.service';

@Component({
  selector: 'app-facebook-audience-filter-dropdown',
  templateUrl: './facebook-audience-filter-dropdown.component.html',
  styleUrls: ['./facebook-audience-filter-dropdown.component.css']
})
export class FacebookAudienceFilterDropdownComponent implements OnInit, AfterViewInit, OnChanges {

  @Input() audience_filter_comp_data: { component, data }; // component and data 

  @ViewChild('container', {
    read: ViewContainerRef,
    static: true
  }) container: ViewContainerRef;

  constructor(private componentFactoryResolver: ComponentFactoryResolver) { }

  ngOnInit() {
    this.renderComponent();
  }

  ngAfterViewInit() {
    
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

  }
}
