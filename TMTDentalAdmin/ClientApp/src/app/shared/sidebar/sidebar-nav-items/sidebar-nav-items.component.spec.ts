import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SidebarNavItemsComponent } from './sidebar-nav-items.component';

describe('SidebarNavItemsComponent', () => {
  let component: SidebarNavItemsComponent;
  let fixture: ComponentFixture<SidebarNavItemsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SidebarNavItemsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SidebarNavItemsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
