import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ServiceListSearchDropdownComponent } from './service-list-search-dropdown.component';

describe('ServiceListSearchDropdownComponent', () => {
  let component: ServiceListSearchDropdownComponent;
  let fixture: ComponentFixture<ServiceListSearchDropdownComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ServiceListSearchDropdownComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ServiceListSearchDropdownComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
