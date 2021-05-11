import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DotKhamLineListComponent } from './dot-kham-line-list.component';

describe('DotKhamLineListComponent', () => {
  let component: DotKhamLineListComponent;
  let fixture: ComponentFixture<DotKhamLineListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DotKhamLineListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DotKhamLineListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
